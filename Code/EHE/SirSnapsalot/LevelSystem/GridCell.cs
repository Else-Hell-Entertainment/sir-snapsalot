using EHE.Global.Config;
using Godot;

namespace EHE.LevelSystem
{
    [GlobalClass]
    public partial class GridCell : Node3D
    {
        public enum Direction
        {
            North,
            South,
            East,
            West,
        }

        public int GridId;

        private int _gridScale = SystemConfig.GridScale;

        [Export]
        public Vector2I GridCoordinates;

        [Export]
        public Godot.Collections.Array<CellComponentData> Components;

        public override void _Ready()
        {
            base._Ready();
            if (Components == null)
            {
                Components = new Godot.Collections.Array<CellComponentData>();
            }
            int x = 1 + (int)GlobalPosition.X / _gridScale;
            int y = 1 + (int)GlobalPosition.Z / _gridScale;
            string xStr = x.ToString().PadRight(3, '0');
            string yStr = y.ToString().PadLeft(3, '0');
            string gridIdStr = xStr + yStr;
            GridId = int.Parse(gridIdStr);
        }

        public void AddComponent(CellComponent.Position position, CellComponent component)
        {
            CellComponentData compData = new CellComponentData();
            compData.Position = position;
            compData.Component = component;
            Components.Add(compData);
        }

        /// <summary>
        /// Generates all the cell components that have been added to the cell.
        /// </summary>
        public void GenerateCellCellComponents()
        {
            foreach (var c in Components)
            {
                SpawnCellComponent(c.Position, c.Component);
            }
        }

        /// <summary>
        /// Spawns a component in the cell at the specified position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="component"></param>
        public void SpawnCellComponent(CellComponent.Position position, CellComponent component)
        {
            var componentInstance = (Node3D)component.GetSceneInstance();
            AddChild(componentInstance);

            var sceneOwner = GetTree().CurrentScene;
            if (sceneOwner != null)
            {
                componentInstance.Owner = sceneOwner;
            }

            SetComponentPosition(componentInstance, position);
            SetComponentRotation(componentInstance, position);
        }

        public void ClearCellComponents()
        {
            var children = GetChildren();
            foreach (var child in children)
            {
                child.QueueFree();
            }

            Components.Clear();
        }

        public void SetComponentPosition(Node3D component, CellComponent.Position position)
        {
            Vector3 globalRoot = GlobalPosition;
            switch (position)
            {
                case CellComponent.Position.Floor:
                    component.GlobalPosition = globalRoot + new Vector3(0, 0, 0);
                    break;
                case CellComponent.Position.NorthWall:
                    component.GlobalPosition = globalRoot + new Vector3(0, 0, -1) * _gridScale / 2;
                    break;
                case CellComponent.Position.SouthWall:
                    component.GlobalPosition = globalRoot + new Vector3(0, 0, 1) * _gridScale / 2;
                    break;
                case CellComponent.Position.EastWall:
                    component.GlobalPosition = globalRoot + new Vector3(1, 0, 0) * _gridScale / 2;
                    break;
                case CellComponent.Position.WestWall:
                    component.GlobalPosition = globalRoot + new Vector3(-1, 0, 0) * _gridScale / 2;
                    break;
            }
        }

        public void SetComponentRotation(Node3D component, CellComponent.Position position)
        {
            switch (position)
            {
                case CellComponent.Position.Floor:
                    component.RotationDegrees = new Vector3(0, 0, 0);
                    break;
                case CellComponent.Position.NorthWall:
                    component.RotationDegrees = new Vector3(0, 0, 0);
                    break;
                case CellComponent.Position.SouthWall:
                    component.RotationDegrees = new Vector3(0, 180, 0);
                    break;
                case CellComponent.Position.EastWall:
                    component.RotationDegrees = new Vector3(0, -90, 0);
                    break;
                case CellComponent.Position.WestWall:
                    component.RotationDegrees = new Vector3(0, 90, 0);
                    break;
            }
        }

        /// <summary>
        /// Is it possible to move OUT of the cell INTO the specified direction? This method is used for pathfinding
        /// purposes. For involuntary movement, use IsSolid to check if the cell is impassable.
        /// Note that floor being impassable does not prevent exiting the cell.
        /// Use IsPassableFromDirection to check if movement INTO the cell from a direction is blocked.
        ///
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool IsPassableToDirection(Direction direction)
        {
            foreach (var c in Components)
            {
                CellComponent.Position pos = c.Position;
                CellComponent cell = c.Component;
                if (cell.BlocksMovement)
                {
                    switch (direction)
                    {
                        case Direction.North:
                            if (pos == CellComponent.Position.NorthWall)
                            {
                                return false;
                            }

                            break;
                        case Direction.South:
                            if (pos == CellComponent.Position.SouthWall)
                            {
                                return false;
                            }

                            break;
                        case Direction.East:
                            if (pos == CellComponent.Position.EastWall)
                            {
                                return false;
                            }

                            break;
                        case Direction.West:
                            if (pos == CellComponent.Position.WestWall)
                            {
                                return false;
                            }

                            break;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Is it possible to move INTO the cell FROM the specified direction? If the floor is blocked, movement into the
        /// cell is blocked from any direction.
        /// Use IsPassableToDirection to check if movement out of the cell in a direction is blocked.
        /// This method is used for pathfinding purposes. For involuntary movement, use
        /// IsSolid to check if the cell is impassable.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool IsPassableFromDirection(Direction direction)
        {
            foreach (var c in Components)
            {
                CellComponent.Position pos = c.Position;
                CellComponent cell = c.Component;
                if (cell.BlocksMovement)
                {
                    if (pos == CellComponent.Position.Floor)
                    {
                        return false; // Floor blocks movement into the cell from any direction.
                    }

                    switch (direction)
                    {
                        case Direction.North:
                            if (pos == CellComponent.Position.NorthWall)
                            {
                                return false;
                            }

                            break;
                        case Direction.South:
                            if (pos == CellComponent.Position.SouthWall)
                            {
                                return false;
                            }

                            break;
                        case Direction.East:
                            if (pos == CellComponent.Position.EastWall)
                            {
                                return false;
                            }

                            break;
                        case Direction.West:
                            if (pos == CellComponent.Position.WestWall)
                            {
                                return false;
                            }

                            break;
                    }
                }
            }

            return true;
        }
    }
}
