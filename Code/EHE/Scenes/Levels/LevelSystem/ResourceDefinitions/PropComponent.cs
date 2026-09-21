using Godot;

namespace EHE.LevelSystem
{
    [GlobalClass]
    public partial class PropComponent : CellComponent
    {
        public override CellComponentType ComponentType => CellComponentType.Prop;
    }
}
