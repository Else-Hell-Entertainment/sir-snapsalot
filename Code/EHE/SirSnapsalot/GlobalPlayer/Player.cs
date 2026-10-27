// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

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
