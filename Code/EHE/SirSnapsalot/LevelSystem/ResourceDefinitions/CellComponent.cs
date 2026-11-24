// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

using Godot;
using Godot.Collections;

namespace EHE.SirSnapsalot.LevelSystem
{
    [GlobalClass]
    public partial class CellComponent : Resource
    {
        public enum CellComponentType
        {
            None,
            Floor,
            Wall,
            Trap,
            Prop,
            Navigation,
        }

        public enum Position
        {
            None,
            Floor,
            NorthWall,
            SouthWall,
            EastWall,
            WestWall,
        }

        [Export]
        public string Name;

        [Export]
        private Array<PackedScene> _scenes = new Array<PackedScene>();

        /// <summary>
        /// Does this component block movement completely, such as a wall? Character can never be moved through a solid
        /// component under any circumstances.
        /// </summary>
        [Export]
        public bool IsSolid = false;

        /// <summary>
        /// Does this component block pathfinding? Some components may be impassable but not solid, such as a trap pit that
        /// blocks regular movement but character may still fall into it.
        /// </summary>
        [Export]
        public bool BlocksMovement = false;

        // TODO: Could this be exported?
        // -> Makes modifying existing components easier in the editor.
        // -> However, might make rotating walls etc. harder to implement if common rotation method is added.
        public virtual CellComponentType ComponentType => CellComponentType.None;

        // TODO: Maybe add exported RequiresFloor property?
        // -> Makes it easier to check whether the placement is valid.
        // See LevelGrid.PlaceGhostComponent

        public Node GetSceneInstance()
        {
            if (_scenes == null || _scenes.Count == 0)
            {
                return null;
            }

            int index = 0;

            if (_scenes.Count > 1)
            {
                index = GD.RandRange(from: 0, to: _scenes.Count - 1);
            }

            return _scenes[index].Instantiate();
        }
    }
}
