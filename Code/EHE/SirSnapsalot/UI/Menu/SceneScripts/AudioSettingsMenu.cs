using EHE.Global.Managers;
using Godot;
using StateKeys = EHE.Global.SceneManagement.SceneCollections.MenuScenes.Keys;

namespace EHE.UI.Menu
{
    public partial class AudioSettingsMenu : MenuScene
    {
        [Export]
        private Button _backToSettingsButton;

        [Export]
        private Button _backToMainMenuButton;

        [Export]
        private Button _resetDefaultsButton;

        [Export]
        private HSlider _masterVolumeSlider;

        [Export]
        private HSlider _musicVolumeSlider;

        [Export]
        private HSlider _sfxVolumeSlider;

        [Export]
        private Label _masterVolumeLabel;

        [Export]
        private Label _musicVolumeLabel;

        [Export]
        private Label _sfxVolumeLabel;

        public override void _Ready()
        {
            ConnectSignals();
            SetSliderPositions();
            UpdateLabelValues();
            if (GameManager.Instance.CurrentState == GameState.Paused)
            {
                _backToMainMenuButton.Visible = false;
            }
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            SettingsManager.Instance.SaveSettings();
            DisconnectSignals();
        }

        private void ConnectSignals()
        {
            _backToSettingsButton.Pressed += OnBackToSettingsPressed;
            _backToMainMenuButton.Pressed += OnBackToMainMenuPressed;
            _masterVolumeSlider.ValueChanged += OnMasterVolumeChanged;
            _musicVolumeSlider.ValueChanged += OnMusicVolumeChanged;
            _sfxVolumeSlider.ValueChanged += OnSFXVolumeChanged;
            _resetDefaultsButton.Pressed += OnResetDefaultsPressed;
        }

        private void DisconnectSignals()
        {
            _backToSettingsButton.Pressed -= OnBackToSettingsPressed;
            _backToMainMenuButton.Pressed -= OnBackToMainMenuPressed;
            _masterVolumeSlider.ValueChanged -= OnMasterVolumeChanged;
            _musicVolumeSlider.ValueChanged -= OnMusicVolumeChanged;
            _sfxVolumeSlider.ValueChanged -= OnSFXVolumeChanged;
            _resetDefaultsButton.Pressed -= OnResetDefaultsPressed;
        }

        private void OnResetDefaultsPressed()
        {
            SettingsManager.Instance.ResetAudioToDefaults();
            SetSliderPositions();
            UpdateLabelValues();
        }

        private void OnSaveSettingsPressed()
        {
            SettingsManager.Instance.SaveSettings();
        }

        private void OnMasterVolumeChanged(double value)
        {
            SettingsManager.Instance.ApplyVolume(AudioManager.MasterBusName, (int)value);
            UpdateLabelValues();
        }

        private void OnMusicVolumeChanged(double value)
        {
            SettingsManager.Instance.ApplyVolume(AudioManager.MusicBusName, (int)value);
            UpdateLabelValues();
        }

        private void OnSFXVolumeChanged(double value)
        {
            SettingsManager.Instance.ApplyVolume(AudioManager.SFXBusName, (int)value);
            UpdateLabelValues();
        }

        private void OnBackToMainMenuPressed()
        {
            RequestTransition(StateKeys.MainMenu);
        }

        private void OnBackToSettingsPressed()
        {
            ReturnToPrevious();
        }

        private void SetSliderPositions()
        {
            string masterBus = AudioManager.MasterBusName;
            string musicBus = AudioManager.MusicBusName;
            string sfxBus = AudioManager.SFXBusName;
            double masterVolume = SettingsManager.Instance.Settings.AudioSettings.BusVolumes[masterBus];
            double musicVolume = SettingsManager.Instance.Settings.AudioSettings.BusVolumes[musicBus];
            double sfxVolume = SettingsManager.Instance.Settings.AudioSettings.BusVolumes[sfxBus];
            _masterVolumeSlider.SetValueNoSignal(masterVolume);
            _musicVolumeSlider.SetValueNoSignal(musicVolume);
            _sfxVolumeSlider.SetValueNoSignal(sfxVolume);
        }

        private void UpdateLabelValues()
        {
            _masterVolumeLabel.Text = $"{_masterVolumeSlider.Value} %";
            _musicVolumeLabel.Text = $"{_musicVolumeSlider.Value} %";
            _sfxVolumeLabel.Text = $"{_sfxVolumeSlider.Value} %";
        }
    }
}
