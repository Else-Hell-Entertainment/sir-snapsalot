using EHE.Global.Config;
using EHE.Global.Logging;
using EHE.Global.SceneManagement;
using EHE.Global.UI;
using EHE.UI.Menu;
using EHE.UI.PlayerUI;
using Godot;
using MenuKeys = EHE.Global.SceneManagement.SceneCollections.MenuScenes.Keys;

namespace EHE.Global.Managers
{
    public partial class UIManager : Node
    {
        public UIManager()
        {
            Name = "UIManager";
        }

        public UIEvents UIEvents { get; } = new UIEvents();

        private CanvasLayer _hudLayer;
        private CanvasLayer _menuLayer;

        private MenuRoot _titleMenu;
        private MenuRoot _pauseMenu;
        private PlayerHud _playerHud;

        private SceneCollection _menuSceneCollection;

        public override void _Ready()
        {
            _hudLayer = new CanvasLayer();
            _hudLayer.Name = "HudLayer";
            AddChild(_hudLayer);
            _menuLayer = new CanvasLayer();
            _menuLayer.Name = "MenuLayer";
            AddChild(_menuLayer);
            ConnectSignals();

            _menuSceneCollection = GD.Load<SceneCollection>(SceneCollections.MenuScenes.CollectionResourcePath);

            this.LogDebug("UI manager ready!");
        }

        public override void _ExitTree()
        {
            DisconnectSignals();
        }

        public void CreatePlayerHud()
        {
            var scene = GD.Load<PackedScene>(GlobalScenePaths.Menu.PlayerHud);
            _playerHud = (PlayerHud)scene.Instantiate();
            _hudLayer.AddChild(_playerHud);
            TogglePlayerHud(true);
        }

        public void DestroyPlayerHud()
        {
            if (_playerHud == null)
            {
                return;
            }

            _hudLayer.RemoveChild(_playerHud);
            _playerHud.QueueFree();
        }

        public void TogglePlayerHud(bool isVisible)
        {
            _playerHud.Visible = isVisible;
        }

        public void OpenTitleMenu()
        {
            var scene = GD.Load<PackedScene>(_menuSceneCollection.GetScenePath(MenuKeys.MainMenuRoot));
            _titleMenu = (MenuRoot)scene.Instantiate();
            _menuLayer.AddChild(_titleMenu);
        }

        public void CloseTitleMenu()
        {
            if (_titleMenu == null)
            {
                return;
            }

            _menuLayer.RemoveChild(_titleMenu);
            _titleMenu.QueueFree();
        }

        public void OpenPauseMenu()
        {
            GD.Print("Opening pause menu");
            var scene = GD.Load<PackedScene>(_menuSceneCollection.GetScenePath(MenuKeys.PauseMenuRoot));
            _pauseMenu = (MenuRoot)scene.Instantiate();
            _menuLayer.AddChild(_pauseMenu);
        }

        public void ClosePauseMenu()
        {
            _pauseMenu.QueueFree();
        }

        private void ConnectSignals()
        {
            this.LogDebug("Connecting UI events.");
            UIEvents.PauseGame += OnPauseGame;
            UIEvents.UnpauseGame += OnUnpauseGame;
        }

        private void DisconnectSignals()
        {
            this.LogDebug("Disconnecting UI events.");
            UIEvents.PauseGame -= OnPauseGame;
            UIEvents.UnpauseGame += OnUnpauseGame;
        }

        private void OnPauseGame()
        {
            OpenPauseMenu();
        }

        private void OnUnpauseGame()
        {
            ClosePauseMenu();
        }
    }
}
