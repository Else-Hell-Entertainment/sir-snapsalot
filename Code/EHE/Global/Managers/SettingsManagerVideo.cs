using System.Collections.Generic;
using EHE.Global.Logging;
using EHE.Global.Settings;
using Godot;

namespace EHE.Global.Managers
{
    public enum AspectRatio
    {
        UnknownAspect,
        Aspect4X3,
        Aspect16X9,
        Aspect16X10,
        Aspect21X9,
        Aspect32X9,
    }

    public struct SupportedResolution(int width, int height, AspectRatio aspectRatio)
    {
        public readonly int Width = width;
        public readonly int Height = height;
        public readonly Vector2I Resolution = new(width, height);
        public readonly AspectRatio AspectRatio = aspectRatio;
    }

    /// <summary>
    /// Manages video settings for the game, including resolution and display mode. Provides methods to apply settings,
    /// detect native resolution, and retrieve supported resolutions.
    /// </summary>
    public partial class SettingsManager
    {
        // NOTE: Settings are stored in the Settings property within the SettingsManager.cs part of the class.

        private SettingsData.VideoSettingsData _videoSettingsData;

        #region Public API

        /// <summary>
        /// Applies the specified resolution to the game window and updates the settings data accordingly.
        /// </summary>
        /// <param name="resolution">Vector2I representing the resolution to apply</param>
        public void ApplyResolution(Vector2I resolution)
        {
            DisplayServer.WindowSetSize(resolution);
            Settings.VideoSettings.ResolutionWidth = resolution.X;
            Settings.VideoSettings.ResolutionHeight = resolution.Y;
            this.LogInfo($"Resolution set to {resolution.X}x{resolution.Y}");
        }

        /// <summary>
        /// Sets the display mode (windowed, fullscreen, exclusive fullscreen) based on the provided mode.
        /// Also refreshes the resolution after changing the display mode to ensure it is applied correctly in windowed mode.
        /// </summary>
        /// <param name="mode">The display mode to set</param>
        public void SetDisplayMode(SettingsData.VideoSettingsData.DispMode mode)
        {
            switch (mode)
            {
                case SettingsData.VideoSettingsData.DispMode.Windowed:
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                    this.LogInfo("Display mode set to windowed.");
                    break;
                case SettingsData.VideoSettingsData.DispMode.Fullscreen:
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                    this.LogInfo("Display mode set to fullscreen.");
                    break;
                case SettingsData.VideoSettingsData.DispMode.ExclusiveFullscreen:
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
                    this.LogInfo("Display mode set to exclusive fullscreen.");
                    break;
                default:
                    this.LogWarning($"Unknown display mode: {mode}. Defaulting to windowed.");
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                    break;
            }

            // Refresh the resolution after changing the display mode to ensure it is applied correctly in windowed mode.
            ApplyResolution(
                new Vector2I(Settings.VideoSettings.ResolutionWidth, Settings.VideoSettings.ResolutionHeight)
            );

            Settings.VideoSettings.DisplayMode = mode;
        }

        /// <summary>
        /// Detects the native resolution of the current display and returns it as a SupportedResolution object,
        /// including its aspect ratio.
        /// </summary>
        /// <returns></returns>
        public SupportedResolution DetectNativeResolution()
        {
            int currentScreen = DisplayServer.WindowGetCurrentScreen();
            Vector2I nativeSize = DisplayServer.ScreenGetSize(currentScreen);
            float aspectRatio = (float)nativeSize.X / nativeSize.Y;
            SupportedResolution detectedResolution;
            if (aspectRatio >= 1.3f && aspectRatio <= 1.4f)
            {
                detectedResolution = new SupportedResolution(nativeSize.X, nativeSize.Y, AspectRatio.Aspect4X3);
            }
            else if (aspectRatio >= 1.7f && aspectRatio <= 1.8f)
            {
                detectedResolution = new SupportedResolution(nativeSize.X, nativeSize.Y, AspectRatio.Aspect16X9);
            }
            else if (aspectRatio >= 1.5f && aspectRatio <= 1.69f)
            {
                detectedResolution = new SupportedResolution(nativeSize.X, nativeSize.Y, AspectRatio.Aspect16X10);
            }
            else if (aspectRatio >= 2.3f && aspectRatio <= 2.4f)
            {
                detectedResolution = new SupportedResolution(nativeSize.X, nativeSize.Y, AspectRatio.Aspect21X9);
            }
            else if (aspectRatio >= 3.5f && aspectRatio <= 3.6f)
            {
                detectedResolution = new SupportedResolution(nativeSize.X, nativeSize.Y, AspectRatio.Aspect32X9);
            }
            else
            {
                detectedResolution = new SupportedResolution(nativeSize.X, nativeSize.Y, AspectRatio.UnknownAspect);
            }

            this.LogInfo(
                "Detecting native resolution and aspect ratio: "
                    + nativeSize.X
                    + "x"
                    + nativeSize.Y
                    + " Aspect Ratio: "
                    + detectedResolution.AspectRatio
            );

            return detectedResolution;
        }

        /// <summary>
        /// Returns a list of supported resolutions with their corresponding aspect ratios. This list can be used to
        /// populate resolution selection menus in the settings UI.
        /// </summary>
        /// <returns></returns>
        public List<SupportedResolution> GetSupportedResolutions()
        {
            return new List<SupportedResolution>
            {
                new(800, 600, AspectRatio.Aspect4X3),
                new(1024, 768, AspectRatio.Aspect4X3),
                new(1280, 720, AspectRatio.Aspect16X9),
                new(1366, 768, AspectRatio.Aspect16X9),
                new(1600, 900, AspectRatio.Aspect16X9),
                new(1920, 1080, AspectRatio.Aspect16X9),
                new(2560, 1440, AspectRatio.Aspect16X9),
                new(3840, 2160, AspectRatio.Aspect16X9),
                new(1280, 800, AspectRatio.Aspect16X10),
                new(1440, 900, AspectRatio.Aspect16X10),
                new(1680, 1050, AspectRatio.Aspect16X10),
                new(1920, 1200, AspectRatio.Aspect16X10),
                new(2560, 1600, AspectRatio.Aspect16X10),
                new(2560, 1080, AspectRatio.Aspect21X9),
                new(3440, 1440, AspectRatio.Aspect21X9),
                new(5120, 1440, AspectRatio.Aspect32X9),
            };
        }

        /// <summary>
        /// Resets video settings to default values based on the native resolution of the display.
        /// Does not save the settings to disk; call SaveSettings() after this method if you want to persist the changes.
        /// </summary>
        public void ResetVideoToDefaults()
        {
            SupportedResolution defaultResolution = DetectNativeResolution();
            Settings.VideoSettings.ResolutionHeight = defaultResolution.Height;
            Settings.VideoSettings.ResolutionWidth = defaultResolution.Width;
            Settings.VideoSettings.DisplayMode = SettingsData.VideoSettingsData.DispMode.Fullscreen;

            ApplyVideoSettings();
        }

        #endregion Public API


        #region Private Methods

        private void ApplyVideoSettings()
        {
            Vector2I newResolution = new Vector2I(
                Settings.VideoSettings.ResolutionWidth,
                Settings.VideoSettings.ResolutionHeight
            );

            SetDisplayMode(Settings.VideoSettings.DisplayMode); // Also refreshes resolution.
        }

        #endregion Private Methods
    }
}
