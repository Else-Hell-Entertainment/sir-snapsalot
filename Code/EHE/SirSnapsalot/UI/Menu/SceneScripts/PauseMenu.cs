using EHE.Global.Config;
using EHE.Global.Managers;
using EHE.Global.SceneManagement;
using Godot;
using StateKeys = EHE.Global.SceneManagement.SceneCollections.MenuScenes.Keys;

namespace EHE.UI.Menu
{
    public partial class PauseMenu : MenuScene
    {
        [Export]
        private Button _resumeButton;

        [Export]
        private Button _saveButton;

        [Export]
        private Button _loadButton;

        [Export]
        private Button _settingsButton;

        [Export]
        private Button _quitToMainButton;

        [Export]
        private Button _quitToDesktopButton;

        public override void _Ready()
        {
            GrabFocus();
            ConnectSignals();
        }

        public override void _ExitTree()
        {
            ReleaseFocus();
            DisconnectSignals();
        }

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed(InputConfig.ESCAPE))
            {
                GD.Print("Pressed Escape in PauseMenu. Resuming game.");
                OnResumePressed();
                AcceptEvent();
            }
        }

        private void ConnectSignals()
        {
            _resumeButton.Pressed += OnResumePressed;
            _saveButton.Pressed += OnSavePressed;
            _loadButton.Pressed += OnLoadPressed;
            _settingsButton.Pressed += OnSettingsPressed;
            _quitToMainButton.Pressed += OnQuitToMainPressed;
            _quitToDesktopButton.Pressed += OnQuitToDesktopPressed;
        }

        private void DisconnectSignals()
        {
            _resumeButton.Pressed -= OnResumePressed;
            _saveButton.Pressed -= OnSavePressed;
            _loadButton.Pressed -= OnLoadPressed;
            _settingsButton.Pressed -= OnSettingsPressed;
            _quitToMainButton.Pressed -= OnQuitToMainPressed;
            _quitToDesktopButton.Pressed -= OnQuitToDesktopPressed;
        }

        private void OnResumePressed()
        {
            GameManager.Instance.UIManager.UIEvents.RaiseUnpauseGame();
        }

        private void OnSavePressed()
        {
            GameManager.Instance.SaveManager.QuickSave();
        }

        private void OnLoadPressed() { }

        private void OnSettingsPressed()
        {
            RequestTransition(StateKeys.Settings);
        }

        private void OnQuitToMainPressed()
        {
            GameManager.Instance.UIManager.UIEvents.RaiseUnpauseGame();
            SceneManager.Instance.RequestSceneTransition(new SceneTransition(GlobalScenePaths.Menu.TitleScreen));

            GameManager.Instance.RequestStateTransition(GameState.TitleScreen);
        }

        private void OnQuitToDesktopPressed()
        {
            GameManager.Instance.QuitToDesktop();
        }
    }
}
