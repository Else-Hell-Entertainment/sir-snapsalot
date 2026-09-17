using EHE.Global.Logging;
using Godot;

namespace EHE.Global.Managers
{
    public partial class AudioManager : Node
    {
        #region Singleton

        public static AudioManager Instance { get; private set; }

        public AudioManager()
        {
            if (Instance == null)
            {
                Instance = this;
                Name = "Audio Manager";
            }
            else if (Instance != this)
            {
                this.LogError("Duplicate instance of Audio Manager! This should not happen.");
                QueueFree();
                return;
            }

            ProcessMode = ProcessModeEnum.Always;

            this.LogDebug("Instance created successfully.");

            CallDeferred(MethodName.Initialize);
        }

        #endregion Singleton

        #region Config

        public const string MasterBusName = "Master";
        public const string MusicBusName = "Music";
        public const string SFXBusName = "SFX";

        #endregion Config

        private AudioStreamPlayer _musicPlayer1;
        private AudioStreamPlayer _musicPlayer2;

        // Use this to play generic sounds that can appear from anywhere in the game like notifications etc.
        // UI sounds should have their own players within the UI scenes.
        private AudioStreamPlayer _genericPlayer;

        private void Initialize()
        {
            // Initialization logic for AudioManager

            this.LogInfo("AudioManager initialized.");
        }

        public override void _Ready()
        {
            // base._Ready();
            // _musicPlayer1 = new AudioStreamPlayer();
            // _musicPlayer1.SetBus(MusicBusName);
            // _musicPlayer1.Name = "MusicPlayer1";
            // AddChild(_musicPlayer1);
            // _musicPlayer2 = new AudioStreamPlayer();
            // _musicPlayer2.SetBus(MusicBusName);
            // _musicPlayer2.Name = "MusicPlayer2";
            // AddChild(_musicPlayer2);
            // _genericPlayer = new AudioStreamPlayer();
            // _genericPlayer.SetBus(SFXBusName);
            // _genericPlayer.Name = "GenericPlayer";
            // AddChild(_genericPlayer);
        }

        /// <summary>
        /// Sets the volume of the specified audio bus to the given linear volume value as a percentage.
        /// </summary>
        /// <param name="audioBus">Which audio bus volume to change.</param>
        /// <param name="volumePercentage">The volume percentage to set (0 = muted, 100 = full volume).</param>
        public bool SetLinearVolume(string audioBus, int volumePercentage)
        {
            // int busIndex = GetBusIndex(audioBus);
            // float linearVolume = Mathf.Clamp(volumePercentage / 100f, 0f, 1f);
            //
            // if (busIndex == -1)
            // {
            //     this.LogError($"Invalid bus index for {audioBus}. Cannot set volume.");
            //     return false;
            // }
            //
            // AudioServer.SetBusVolumeLinear(busIndex, linearVolume);
            return true;
        }

        private int GetBusIndex(string audioBus)
        {
            // return audioBus switch
            // {
            //     MasterBusName => AudioServer.GetBusIndex(MasterBusName),
            //     MusicBusName => AudioServer.GetBusIndex(MusicBusName),
            //     SFXBusName => AudioServer.GetBusIndex(SFXBusName),
            //     _ => -1,
            // };
            return -1;
        }
    }
}
