using Godot;

namespace EHE.SirSnapsalot.LevelSystem
{
    [GlobalClass]
    public partial class PropComponent : CellComponent
    {
        public override CellComponentType ComponentType => CellComponentType.Prop;
    }
}
