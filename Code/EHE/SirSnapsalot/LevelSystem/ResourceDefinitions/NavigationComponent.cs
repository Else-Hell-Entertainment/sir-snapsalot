using System;
using Godot;

namespace EHE.LevelSystem
{
    public partial class NavigationComponent : CellComponent
    {
        public override CellComponentType ComponentType => CellComponentType.Navigation;
    }
}
