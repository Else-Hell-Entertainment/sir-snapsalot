using Godot;

namespace EHE.LevelSystem
{
    [GlobalClass]
    public partial class CellComponent : Resource
    {
        public enum CellComponentType
        {
            NONE,
            Floor,
            Wall,
            Trap,
            Prop,
            Navigation,
        }

        public enum Position
        {
            NONE,
            Floor,
            NorthWall,
            SouthWall,
            EastWall,
            WestWall,
        }

        [Export]
        public string Name;

        [Export]
        private PackedScene _scene;

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

        public virtual CellComponentType ComponentType => CellComponentType.NONE;

        public Node GetSceneInstance()
        {
            if (_scene != null)
            {
                return _scene.Instantiate();
            }

            return null;
        }
    }
}
