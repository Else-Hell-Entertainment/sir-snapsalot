using EHE.Global.Logging;
using EHE.Global.Managers;
using EHE.Global.Settings;
using Godot;

namespace EHE.UI.Menu
{
    public partial class VideoSettingsMenu : MenuScene
    {
        [Export]
        private OptionButton _resolutionDropdown;

        [Export]
        private OptionButton _displayModeDropdown;

        [Export]
        private Button _saveSettingsButton;

        [Export]
        private Button _backToSettingsButton;

        [Export]
        private Button _cancelButton;

        [Export]
        private Button _resetDefaultsButton;

        [Export]
        private Control _popupScreen;

        [Export]
        private Button _popupSaveButton;

        [Export]
        private Button _popupCancelButton;

        private bool _unsavedChanges = false;

        public override void _Ready()
        {
            _popupScreen.Visible = false;
            ConnectSignals();

            RefreshSaveButtonState();
            BuildResolutionButton();
            BuildDisplayModeButton();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            DisconnectSignals();
            _unsavedChanges = false;
        }

        private void ConnectSignals()
        {
            _saveSettingsButton.Pressed += OnSaveSettingsButtonPressed;
            _backToSettingsButton.Pressed += OnBackToSettingsButtonPressed;
            _resolutionDropdown.ItemSelected += OnResolutionSelected;
            _displayModeDropdown.ItemSelected += OnDisplayModeSelected;
            _cancelButton.Pressed += OnCancelButtonPressed;
            _resetDefaultsButton.Pressed += OnResetDefaultsButtonPressed;
            _popupSaveButton.Pressed += OnPopupSaveButtonPressed;
            _popupCancelButton.Pressed += OnPopupCancelButtonPressed;
        }

        private void DisconnectSignals()
        {
            _saveSettingsButton.Pressed -= OnSaveSettingsButtonPressed;
            _backToSettingsButton.Pressed -= OnBackToSettingsButtonPressed;
            _resolutionDropdown.ItemSelected -= OnResolutionSelected;
            _displayModeDropdown.ItemSelected -= OnDisplayModeSelected;
            _cancelButton.Pressed -= OnCancelButtonPressed;
            _resetDefaultsButton.Pressed -= OnResetDefaultsButtonPressed;
            _popupSaveButton.Pressed -= OnPopupSaveButtonPressed;
            _popupCancelButton.Pressed -= OnPopupCancelButtonPressed;
        }

        /// <summary>
        /// Set the DisplayMode based on the selected index from the dropdown. The index corresponds to the following modes:
        /// 0 - Windowed
        /// 1 - Fullscreen
        /// 2 - Exclusive Fullscreen
        /// </summary>
        /// <param name="index"></param>
        private void OnDisplayModeSelected(long index)
        {
            switch (index)
            {
                case 0:
                    SettingsManager.Instance.SetDisplayMode(SettingsData.VideoSettingsData.DispMode.Windowed);

                    break;
                case 1:
                    SettingsManager.Instance.SetDisplayMode(SettingsData.VideoSettingsData.DispMode.Fullscreen);

                    break;
                case 2:
                    SettingsManager.Instance.SetDisplayMode(
                        SettingsData.VideoSettingsData.DispMode.ExclusiveFullscreen
                    );

                    break;
                default:
                    this.LogError("Invalid display mode index selected.");
                    break;
            }

            _unsavedChanges = true;
            RefreshSaveButtonState();
        }

        private void OnResolutionSelected(long index)
        {
            var supportedResolutions = SettingsManager.Instance.GetSupportedResolutions();
            if (index >= 0 && index < supportedResolutions.Count)
            {
                var selectedResolution = supportedResolutions[(int)index];
                SettingsManager.Instance.ApplyResolution(selectedResolution.Resolution);
            }

            _unsavedChanges = true;
            RefreshSaveButtonState();
        }

        private void BuildResolutionButton()
        {
            _resolutionDropdown.Clear();
            var supportedResolutions = SettingsManager.Instance.GetSupportedResolutions();
            var nativeResolution = SettingsManager.Instance.DetectNativeResolution();
            foreach (var resolution in supportedResolutions)
            {
                string aspectRatioString = resolution.AspectRatio switch
                {
                    AspectRatio.Aspect4X3 => "4:3",
                    AspectRatio.Aspect16X9 => "16:9",
                    AspectRatio.Aspect16X10 => "16:10",
                    AspectRatio.Aspect21X9 => "21:9",
                    AspectRatio.Aspect32X9 => "32:9",
                    _ => "Unknown",
                };

                string resolutionText = $"{resolution.Width} x {resolution.Height} ({aspectRatioString})";

                if (resolution.Height == nativeResolution.Height && resolution.Width == nativeResolution.Width)
                {
                    resolutionText += " (Native)";
                }

                _resolutionDropdown.AddItem(resolutionText);
            }

            // Set the current resolution as the selected item
            var currentResolution = new Vector2I(
                SettingsManager.Instance.Settings.VideoSettings.ResolutionWidth,
                SettingsManager.Instance.Settings.VideoSettings.ResolutionHeight
            );

            int selectedIndex = supportedResolutions.FindIndex(r => r.Resolution == currentResolution);
            if (selectedIndex >= 0)
            {
                _resolutionDropdown.Selected = selectedIndex;
            }
        }

        private void BuildDisplayModeButton()
        {
            // Set the current display mode as the selected item
            var currentDisplayMode = SettingsManager.Instance.Settings.VideoSettings.DisplayMode;
            int selectedIndex = currentDisplayMode switch
            {
                SettingsData.VideoSettingsData.DispMode.Windowed => 0,
                SettingsData.VideoSettingsData.DispMode.Fullscreen => 1,
                SettingsData.VideoSettingsData.DispMode.ExclusiveFullscreen => 2,
                _ => 0,
            };

            _displayModeDropdown.Selected = selectedIndex;
        }

        private void OnBackToSettingsButtonPressed()
        {
            if (_unsavedChanges)
            {
                _popupScreen.Visible = true;
            }
            else
            {
                ReturnToPrevious();
            }
        }

        private void OnSaveSettingsButtonPressed()
        {
            SettingsManager.Instance.SaveSettings();
            _unsavedChanges = false;
            RefreshSaveButtonState();
        }

        private void OnCancelButtonPressed()
        {
            SettingsManager.Instance.LoadSettings();
            _unsavedChanges = false;
            RefreshSaveButtonState();

            // Rebuild the dropdowns to reflect the reverted settings.
            BuildResolutionButton();
            BuildDisplayModeButton();
        }

        private void OnResetDefaultsButtonPressed()
        {
            SettingsManager.Instance.ResetVideoToDefaults();
            _unsavedChanges = true;
            RefreshSaveButtonState();

            // Rebuild the dropdowns to reflect the default settings.
            BuildResolutionButton();
            BuildDisplayModeButton();
        }

        private void RefreshSaveButtonState()
        {
            _saveSettingsButton.Disabled = !_unsavedChanges;
            _cancelButton.Disabled = !_unsavedChanges;
        }

        private void OnPopupCancelButtonPressed()
        {
            _popupScreen.Visible = false;
            OnCancelButtonPressed();
        }

        private void OnPopupSaveButtonPressed()
        {
            _popupScreen.Visible = false;
            OnSaveSettingsButtonPressed();
            ReturnToPrevious();
        }
    }
}
