using Godot;
using StateKeys = EHE.Global.SceneManagement.SceneCollections.MenuScenes.Keys;

namespace EHE.UI.Menu
{
    public partial class SettingsMenu : MenuScene
    {
        [Export]
        private Button _audioSettingsButton;

        [Export]
        private Button _videoSettingsButton;

        [Export]
        private Button _gameplaySettingsButton;

        [Export]
        private Button _backButton;

        public override void _Ready()
        {
            _audioSettingsButton.Pressed += OnAudioSettingsButtonPressed;
            _videoSettingsButton.Pressed += OnVideoSettingsButtonPressed;
            _gameplaySettingsButton.Pressed += OnGameplaySettingsPressed;
            _backButton.Pressed += OnBackButtonPressed;
        }

        private void OnGameplaySettingsPressed()
        {
            RequestTransition(StateKeys.GameplaySettings);
        }

        private void OnVideoSettingsButtonPressed()
        {
            RequestTransition(StateKeys.VideoSettings);
        }

        private void OnBackButtonPressed()
        {
            ReturnToPrevious();
        }

        private void OnAudioSettingsButtonPressed()
        {
            RequestTransition(StateKeys.AudioSettings);
        }
    }
}
