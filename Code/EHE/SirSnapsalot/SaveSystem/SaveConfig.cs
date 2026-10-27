// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

using Godot;

namespace EHE.SirSnapsalot.SaveSystem
{
    [GlobalClass]
    public partial class SaveConfig : Resource
    {
        [Export]
        public SaveBackend DefaultSaveBackend = SaveBackend.None;

        [Export]
        public string SaveLocation = "user://save";

        [Export]
        public string QuickSaveSlot = "QuickSave";
    }
}
