using Godot;

namespace EHE.SirSnapsalot.LevelSystem
{
    [GlobalClass]
    public partial class FloorComponent : CellComponent
    {
        public override CellComponentType ComponentType => CellComponentType.Floor;
    }
}
