using EHE.SirSnapsalot.SaveSystem;

namespace EHE.SirSnapsalot.GlobalPlayer
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
