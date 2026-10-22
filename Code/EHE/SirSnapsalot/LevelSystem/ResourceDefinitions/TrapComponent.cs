using Godot;

namespace EHE.SirSnapsalot.LevelSystem
{
    [GlobalClass]
    public partial class TrapComponent : CellComponent
    {
        public override CellComponentType ComponentType => CellComponentType.Trap;
    }
}
