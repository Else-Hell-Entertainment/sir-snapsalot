// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

using Godot;

namespace EHE.SirSnapsalot.LevelSystem
{
    [GlobalClass]
    public partial class PropComponent : CellComponent
    {
        public override CellComponentType ComponentType => CellComponentType.Prop;
    }
}
