using Godot;

namespace EHE.Global.SaveSystem
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
