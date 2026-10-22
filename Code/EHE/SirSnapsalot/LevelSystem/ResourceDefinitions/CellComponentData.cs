using Godot;

namespace EHE.SirSnapsalot.LevelSystem
{
    [GlobalClass]
    public partial class CellComponentData : Resource
    {
        [Export]
        public CellComponent.Position Position { get; set; }

        [Export]
        public CellComponent Component { get; set; }
    }
}
