using System;
using System.IO;
using System.Text.Json;
using EHE.Global.Logging;
using EHE.Global.Settings;
using Godot;
using FileAccess = System.IO.FileAccess;

namespace EHE.Global.Managers
{
    /// <summary>
    /// Manages the game's runtime configuration and persists it to the user's local Godot data directory.
    /// </summary>
    /// <remarks>
    /// This class is intended to run as an autoload singleton. The canonical instance owns the
    /// in-memory <see cref="SettingsData"/> and writes it to <c>user://settings.json</c>
    /// so the configuration remains local to the current user profile and project instance.
    /// </remarks>
    public partial class SettingsManager : Node
    {
        #region Singleton

        public static SettingsManager Instance { get; private set; }

        public SettingsManager()
        {
            if (Instance == null)
            {
                Instance = this;
                Name = "Settings Manager";
            }
            else if (Instance != this)
            {
                this.LogError("Duplicate instance of Settings Manager! This should not happen.");
                QueueFree();
                return;
            }

            ProcessMode = ProcessModeEnum.Always;

            this.LogDebug("Instance created successfully.");

            CallDeferred(MethodName.Initialize);
        }

        private void Initialize()
        {
            GD.Print("[Settings Manager] Initializing settings manager.");
            LoadSettings();
        }

        #endregion Singleton

        #region Properties

        /// <summary>
        /// The current settings data. This is the data that is being modified by the user in the settings menu
        /// and used by the game when running.
        /// </summary>
        public SettingsData Settings { get; private set; } = new();

        #endregion

        #region Private fields

        // Store settings in the user's project-local data directory so the config remains
        // scoped to the current machine/user profile and does not rely on project-relative
        // files that may be shared or overwritten.
        private const string SettingsFilePath = "user://settings.json";

        #endregion Private fields


        #region Save and Load Functionality

        /// <summary>
        /// Saves the current in-memory settings to disk using the configured user-scoped
        /// settings file path.
        /// </summary>
        public void SaveSettings()
        {
            string path = ProjectSettings.GlobalizePath(SettingsFilePath);
            JsonSerializerOptions options = new() { WriteIndented = true };

            using FileStream fileStream = File.Open(path, FileMode.Create, FileAccess.Write);

            JsonSerializer.Serialize(fileStream, Settings, options);
            this.LogInfo($"Settings saved to {path}.");
        }

        /// <summary>
        /// Loads settings from disk if present; otherwise initializes the default
        /// configuration and persists it.
        /// </summary>
        public void LoadSettings()
        {
            string path = ProjectSettings.GlobalizePath(SettingsFilePath);
            if (!File.Exists(path))
            {
                this.LogWarning(
                    $"Settings file not found at {path}. If the game is running for the first time, this is "
                        + $"expected. Creating default settings."
                );

                ResetToDefaultSettings();
                return;
            }

            JsonSerializerOptions options = new() { WriteIndented = true };
            try
            {
                using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    SettingsData data = JsonSerializer.Deserialize<SettingsData>(fileStream, options);
                    Settings = data;
                }

                this.LogInfo($"Settings loaded from {path}.");
            }
            catch (Exception e)
            {
                this.LogError(
                    $"Failed to load settings from {path}. Likely due to corrupted file. Creating new default "
                        + $"settings file and overwriting corrupted data."
                );

                ResetToDefaultSettings();
            }

            ApplySettings();
        }

        #endregion Save and Load Functionality

        #region Private Methods

        private void ApplySettings()
        {
            ApplyVideoSettings();
            ApplyAudioSettings();
            ApplyGameplaySettings();
            ApplyControlSettings();
        }

        /// <summary>
        /// Resets all settings to their default values and saves them to disk.
        /// This is typically called when the user wants to revert all settings to
        /// their original state or when program is run for the first time.
        /// </summary>
        private void ResetToDefaultSettings()
        {
            Settings = new SettingsData();
            ResetVideoToDefaults();
            ApplyAudioSettings();
            SaveSettings();
        }

        #endregion Private Methods
    }
}
