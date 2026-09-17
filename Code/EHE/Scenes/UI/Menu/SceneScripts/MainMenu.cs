using EHE.Global.Logging;
using EHE.Global.Managers;
using Godot;
using StateKeys = EHE.Global.SceneManagement.SceneCollections.MenuScenes.Keys;

namespace EHE.UI.Menu
{
    public partial class MainMenu : MenuScene
    {
        [Export]
        private Button _startNewGameButton;

        [Export]
        private Button _settingsButton;

        [Export]
        private Button _saveButton;

        [Export]
        private Button _loadButton;

        [Export]
        private Button _creditsButton;

        [Export]
        private Button _exitButton;

        public override void _Ready()
        {
            this.LogDebug("MainMenu ready.");
            ConnectSignals();
        }

        private void ConnectSignals()
        {
            _startNewGameButton.Pressed += OnStartNewGameButtonPressed;
            _settingsButton.Pressed += OnSettingsButtonPressed;
            _saveButton.Pressed += OnSaveButtonPressed;
            _loadButton.Pressed += OnLoadButtonPressed;
            _creditsButton.Pressed += OnCreditsButtonPressed;
            _exitButton.Pressed += OnExitButtonPressed;
        }

        private void OnStartNewGameButtonPressed()
        {
            GameManager.Instance.StartGame();
        }

        private void OnSettingsButtonPressed()
        {
            RequestTransition(StateKeys.Settings);
        }

        private void OnSaveButtonPressed()
        {
            GD.Print("Save button pressed.");
        }

        private void OnLoadButtonPressed()
        {
            GameManager.Instance.LoadGame();
        }

        private void OnCreditsButtonPressed()
        {
            GD.Print("Credits button pressed.");
        }

        private void OnExitButtonPressed()
        {
            GetTree().Quit();
        }
    }
}
