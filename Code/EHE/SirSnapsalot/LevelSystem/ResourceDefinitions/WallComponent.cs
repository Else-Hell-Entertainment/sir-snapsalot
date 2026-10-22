using Godot;

namespace EHE.LevelSystem
{
    [GlobalClass]
    public partial class WallComponent : CellComponent
    {
        public override CellComponentType ComponentType => CellComponentType.Wall;
    }
}
