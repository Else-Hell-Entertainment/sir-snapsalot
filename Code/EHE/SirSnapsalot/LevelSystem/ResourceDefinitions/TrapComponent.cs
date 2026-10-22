using Godot;

namespace EHE.LevelSystem
{
    [GlobalClass]
    public partial class TrapComponent : CellComponent
    {
        public override CellComponentType ComponentType => CellComponentType.Trap;
    }
}
