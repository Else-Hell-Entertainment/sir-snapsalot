using System.Collections.Generic;
using EHE.Global.Managers;
using Godot;

namespace EHE.Global.Settings
{
    public class SettingsData
    {
        public VideoSettingsData VideoSettings { get; set; } = new();

        public AudioSettingsData AudioSettings { get; set; } = new();

        public GameplaySettingsData GameplaySettings { get; set; } = new();

        public ControlSettingsData ControlSettings { get; set; } = new();

        public class VideoSettingsData
        {
            public enum DispMode
            {
                Windowed,
                Fullscreen,
                ExclusiveFullscreen,
            }

            public int ResolutionWidth { get; set; } = 1920;

            public int ResolutionHeight { get; set; } = 1080;

            public DispMode DisplayMode { get; set; } = DispMode.Fullscreen;
        }

        public class AudioSettingsData
        {
            #region Godot audio settings

            // Percentage values for Godot audio settings, ranging from 0 to 100.

            public Dictionary<string, int> BusVolumes { get; set; } =
                new()
                {
                    { AudioManager.MasterBusName, 100 },
                    { AudioManager.MusicBusName, 100 },
                    { AudioManager.SFXBusName, 100 },
                };

            #endregion Godot audio settings
        }

        public class GameplaySettingsData
        {
            public string Difficulty { get; set; } = "Incredible";
        }

        public class ControlSettingsData
        {
            public Dictionary<string, InputEvent> InputActionMap { get; set; } = new();
        }
    }
}
