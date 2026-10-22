using Godot;

namespace EHE.SirSnapsalot.LevelSystem
{
    [GlobalClass]
    public partial class WallComponent : CellComponent
    {
        public override CellComponentType ComponentType => CellComponentType.Wall;
    }
}
