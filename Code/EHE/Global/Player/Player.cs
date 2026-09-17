using EHE.Global.SaveSystem;

namespace EHE.Global.GlobalPlayer
{
    public class Player : ISaveable<PlayerData>
    {
        public PlayerData SaveState()
        {
            PlayerData data = new PlayerData();
            return data;
        }

        public void LoadState(PlayerData state) { }
    }
}
