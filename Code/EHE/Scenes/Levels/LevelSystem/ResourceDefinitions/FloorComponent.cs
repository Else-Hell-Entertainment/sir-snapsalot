using Godot;

namespace EHE.LevelSystem
{
    [GlobalClass]
    public partial class FloorComponent : CellComponent
    {
        public override CellComponentType ComponentType => CellComponentType.Floor;
    }
}
