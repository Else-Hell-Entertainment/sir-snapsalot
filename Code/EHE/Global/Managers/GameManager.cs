using System.Collections.Generic;
using EHE.Global.Config;
using EHE.Global.FSM;
using EHE.Global.GameStates;
using EHE.Global.Logging;
using EHE.Global.SaveSystem;
using Godot;

namespace EHE.Global.Managers
{
    public enum GameState
    {
        Boot,
        TitleScreen,
        Playing,
        Paused,
    }

    public partial class GameManager : Node
    {
        #region Singleton

        public static GameManager Instance { get; private set; }

        public GameManager()
        {
            if (Instance == null)
            {
                Instance = this;
                Name = "Game Manager";
            }
            else if (Instance != this)
            {
                this.LogError("Duplicate instance of GameManager! This should not happen.");
                QueueFree();
                return;
            }
            ProcessMode = ProcessModeEnum.Always;

            this.LogDebug("Instance created successfully.");

            CallDeferred(MethodName.Initialize);
        }

        private void Initialize()
        {
            CurrentState = GameState.Boot;
            SceneTree = GetTree();
            UIManager = new UIManager();
            AddChild(UIManager);
            SaveConfig saveConfig = GD.Load<SaveConfig>(SystemConfig.SaveConfigPath);
            if (saveConfig != null)
            {
                SaveManager = new SaveManager(saveConfig);
            }
            else
            {
                this.LogFatalError("Failed to load SaveConfig from path: " + SystemConfig.SaveConfigPath);
            }

            _stateInstances.Add(GameState.Boot, new GameStateBoot());
            _stateInstances.Add(GameState.TitleScreen, new GameStateTitleScreen());
            _stateInstances.Add(GameState.Playing, new GameStatePlaying());
            _stateInstances.Add(GameState.Paused, new GameStatePaused());
        }

        #endregion Singleton

        #region Properties

        public SceneTree SceneTree { get; private set; }

        public UIManager UIManager { get; private set; }

        public SaveManager SaveManager { get; private set; }

        public GameState CurrentState { get; private set; }

        #endregion

        #region Fields

        private StateMachine _gameStateMachine = new();
        private Dictionary<GameState, GameStateBase> _stateInstances = new();

        #endregion Fields

        public void RequestStateTransition(GameState newState)
        {
            this.LogDebug($"Current game state: {CurrentState}, Requested new state: {newState}");
            if (IsValidTransition(CurrentState, newState))
            {
                _stateInstances.TryGetValue(newState, out GameStateBase stateInstance);

                if (stateInstance != null)
                {
                    CurrentState = newState;
                    _gameStateMachine.ChangeState(stateInstance);
                }
            }
            else
            {
                this.LogWarning($"Invalid transition from {CurrentState} to {newState}");
            }
        }

        public void StartGame()
        {
            UIManager.CloseTitleMenu();

            GameData gameData = GenerateStartingData();
            SaveManager.LoadGameData(gameData);
            SceneManager.Instance.StartLoadedGame();
            RequestStateTransition(GameState.Playing);
        }

        public void LoadGame()
        {
            SaveManager.QuickLoad();
            UIManager.CloseTitleMenu();
            SceneManager.Instance.StartLoadedGame();
            RequestStateTransition(GameState.Playing);
        }

        public void QuitToDesktop()
        {
            GD.Print("[Game Manager] Quitting to desktop.");
            SceneTree.Quit();
        }

        private bool IsValidTransition(GameState oldState, GameState newState)
        {
            return true;
        }

        private GameData GenerateStartingData()
        {
            GameStartConfig startConfig = new GameStartConfig();
            GameData gameData = new GameData();
            PlayerData playerData = new PlayerData();
            playerData.PlayerId = startConfig.DefaultPlayerId;

            Inventory startingInventory = new Inventory();
            startingInventory.Size = startConfig.StartingInventorySize;
            startingInventory.InitializeInventory();
            foreach (var item in startConfig.StartingItems)
            {
                startingInventory.AddItem(item);
            }
            playerData.InventoryData = startingInventory.SaveState();

            gameData.PlayerData = playerData;

            SceneManagerData sceneManagerData = new SceneManagerData();
            sceneManagerData.CurrentSceneKey = startConfig.DefaultSceneKey;
            gameData.SceneManagerData = sceneManagerData;
            return gameData;
        }
    }
}
