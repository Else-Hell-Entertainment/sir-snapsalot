// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

namespace EHE.SirSnapsalot.SaveSystem
{
    public sealed class GameData
    {
        public PlayerData PlayerData { get; set; }
        public SceneManagerData SceneManagerData { get; set; }
    }

    public sealed class PlayerData
    {
        public string PlayerId { get; set; }
    }

    public sealed class SceneManagerData { }
}
