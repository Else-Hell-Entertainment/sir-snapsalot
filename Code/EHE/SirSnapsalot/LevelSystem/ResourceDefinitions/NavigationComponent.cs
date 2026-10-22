using System;
using Godot;

namespace EHE.SirSnapsalot.LevelSystem
{
    public partial class NavigationComponent : CellComponent
    {
        public override CellComponentType ComponentType => CellComponentType.Navigation;
    }
}
