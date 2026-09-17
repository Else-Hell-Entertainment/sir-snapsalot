using EHE.Global.Logging;

namespace EHE.Global.Managers
{
    public partial class SettingsManager
    {
        /// <summary>
        /// Applies the given volume to the specified audio bus and updates the settings file accordingly.
        /// </summary>
        /// <param name="bus">Bus name to which the setting is applied.</param>
        /// <param name="busVolume">Volume value to apply as percentage (0-100). </param>
        public void ApplyVolume(string bus, int busVolume)
        {
            if (busVolume < 0 || busVolume > 100)
            {
                this.LogError($"Invalid volume value for bus '{bus}': {busVolume}. Volume must be between 0 and 100.");
                return;
            }

            if (AudioManager.Instance.SetLinearVolume(bus, busVolume))
            {
                if (Settings.AudioSettings.BusVolumes.ContainsKey(bus))
                {
                    Settings.AudioSettings.BusVolumes[bus] = busVolume;
                }
                else
                {
                    this.LogError(
                        $"Bus '{bus}' was updated in audio manager but not found in settings! "
                            + "This should not happen, check the bus name and ensure it is defined in the settings."
                    );
                }

                this.LogInfo($"Applied volume for bus '{bus}': {busVolume}%");
            }
            else
            {
                this.LogError($"Failed to apply volume for bus '{bus}': {busVolume}%");
            }
        }

        public void ResetAudioToDefaults()
        {
            Settings.AudioSettings.BusVolumes[AudioManager.MasterBusName] = 100;
            Settings.AudioSettings.BusVolumes[AudioManager.MusicBusName] = 100;
            Settings.AudioSettings.BusVolumes[AudioManager.SFXBusName] = 100;

            ApplyAudioSettings();
        }

        /// <summary>
        /// Applies the audio settings from the settings file to the audio manager, ensuring that all bus volumes are set
        /// correctly. Bus names are fetched from Settings file and must match the bus names defined in the AudioManager.
        /// </summary>
        private void ApplyAudioSettings()
        {
            foreach (var busVolume in Settings.AudioSettings.BusVolumes)
            {
                if (AudioManager.Instance.SetLinearVolume(busVolume.Key, busVolume.Value))
                {
                    this.LogInfo($"Applied volume for bus '{busVolume.Key}': {busVolume.Value}%");
                }
                else
                {
                    this.LogError($"Failed to apply volume for bus '{busVolume.Key}': {busVolume.Value}%");
                }
            }
        }
    }
}
