using EHE.Global.Config;
using EHE.Global.Logging;
using Godot;
using SceneKeys = EHE.Global.SceneManagement.SceneCollections.MenuScenes.Keys;

namespace EHE.UI.Menu
{
    public partial class GameplaySettingsMenu : MenuScene
    {
        [Export]
        private Button _controlsMenuButton;

        [Export]
        private Button _saveChangesButton;

        [Export]
        private Button _backButton;

        public override void _EnterTree()
        {
            base._EnterTree();
            ConnectSignals();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            DisconnectSignals();
        }

        private void ConnectSignals()
        {
            _backButton.Pressed += OnBackButtonPressed;
            _controlsMenuButton.Pressed += OnControlsMenuButtonPressed;
            _saveChangesButton.Pressed += OnSaveChangesButtonPressed;
        }

        private void DisconnectSignals()
        {
            _backButton.Pressed -= OnBackButtonPressed;
            _controlsMenuButton.Pressed -= OnControlsMenuButtonPressed;
            _saveChangesButton.Pressed -= OnSaveChangesButtonPressed;
        }

        private void OnControlsMenuButtonPressed()
        {
            RequestTransition(SceneKeys.ControlSettings);
        }

        private void OnBackButtonPressed()
        {
            ReturnToPrevious();
        }

        private void OnSaveChangesButtonPressed()
        {
            this.LogInfo("Saving gameplay settings changes.");
        }
    }
}
