using System.Threading.Tasks;
using EHE.Global.GlobalPlayer;
using EHE.Global.Logging;
using EHE.Global.SaveSystem;
using Godot;

namespace EHE.Global.Managers
{
    public partial class PlayerManager : Node, ISaveable<PlayerData>
    {
        #region Singleton

        public static PlayerManager Instance { get; private set; }

        public PlayerManager()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                this.LogError("Duplicate instance of PlayerManager! This should not happen.");
                QueueFree();
                return;
            }

            Initialize();
        }

        #endregion Singleton

        #region Properties

        public Player Player { get; private set; }

        public PlayerEvents PlayerEvents { get; private set; }

        public PlayerData PlayerData;

        #endregion Properties


        private void Initialize()
        {
            PlayerEvents = new PlayerEvents();
        }

        public void LoadPlayer()
        {
            GD.Print("[Player Manager] Loading player data!");
            Player.LoadState(PlayerData);
        }

        public async Task UnloadPlayer()
        {
            // TODO: Implement if player needs to be unloaded at some point.
        }

        public void DestroyPlayer()
        {
            if (Player == null)
            {
                this.LogInfo("DestroyPlayer called, but Player is null, cannot destroy.");
                return;
            }
            Player = null;
            GameManager.Instance.UIManager.DestroyPlayerHud();
        }

        public PlayerData SaveState()
        {
            if (Player != null)
            {
                this.LogInfo("Player state saved successfully!");
                PlayerData = Player.SaveState();
            }
            else
            {
                this.LogWarning("SaveState called, but Player is null, cannot save state.");
            }

            return PlayerData;
        }

        public void LoadState(PlayerData state)
        {
            PlayerData = state;
        }
    }
}
