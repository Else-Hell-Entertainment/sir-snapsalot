using System;
using System.Collections.Generic;
using Godot;

namespace EHE.LevelSystem
{
    public partial class GridCell : Node3D
    {
        public GridCell(int GridSize)
        {
            _gridSize = GridSize;
        }

        public enum Direction
        {
            North,
            South,
            East,
            West,
        }

        public int GridId;

        private int _gridSize;

        public List<Tuple<CellComponent.Position, CellComponent>> Components = new();

        public override void _Ready()
        {
            base._Ready();
            int x = 1 + (int)GlobalPosition.X / _gridSize;
            int y = 1 + (int)GlobalPosition.Z / _gridSize;
            string xStr = x.ToString().PadRight(3, '0');
            string yStr = y.ToString().PadLeft(3, '0');
            string gridIdStr = xStr + yStr;
            GridId = int.Parse(gridIdStr);
        }

        public void AddComponent(CellComponent.Position position, CellComponent component)
        {
            Components.Add(new Tuple<CellComponent.Position, CellComponent>(position, component));
            GD.Print("Added component at position: " + position);
            foreach (var com in Components)
            {
                GD.Print("Component at position: " + com.Item1);
            }
        }

        /// <summary>
        /// Generates all the cell components that have been added to the cell.
        /// </summary>
        public void GenerateCellCellComponents()
        {
            foreach (var c in Components)
            {
                SpawnCellComponent(c.Item1, c.Item2);
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
                    component.GlobalPosition = globalRoot + new Vector3(0, 0, -1) * _gridSize / 2;
                    break;
                case CellComponent.Position.SouthWall:
                    component.GlobalPosition = globalRoot + new Vector3(0, 0, 1) * _gridSize / 2;
                    break;
                case CellComponent.Position.EastWall:
                    component.GlobalPosition = globalRoot + new Vector3(1, 0, 0) * _gridSize / 2;
                    break;
                case CellComponent.Position.WestWall:
                    component.GlobalPosition = globalRoot + new Vector3(-1, 0, 0) * _gridSize / 2;
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
                GD.Print("Checking component at position: " + c.Item1 + " for movement to direction: " + direction);
                CellComponent.Position pos = c.Item1;
                CellComponent cell = c.Item2;
                if (cell.BlocksMovement)
                {
                    switch (direction)
                    {
                        case Direction.North:
                            if (pos == CellComponent.Position.NorthWall)
                            {
                                GD.Print("blocking movement to direction: " + direction);
                                return false;
                            }

                            break;
                        case Direction.South:
                            if (pos == CellComponent.Position.SouthWall)
                            {
                                GD.Print("blocking movement to direction: " + direction);
                                return false;
                            }

                            break;
                        case Direction.East:
                            if (pos == CellComponent.Position.EastWall)
                            {
                                GD.Print("blocking movement to direction: " + direction);
                                return false;
                            }

                            break;
                        case Direction.West:
                            if (pos == CellComponent.Position.WestWall)
                            {
                                GD.Print("blocking movement to direction: " + direction);
                                return false;
                            }

                            break;
                    }
                }
            }

            GD.Print("Movement to direction: " + direction + " is passable.");
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
                CellComponent.Position pos = c.Item1;
                CellComponent cell = c.Item2;
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
                                GD.Print("blocking movement from direction: " + direction);
                                return false;
                            }

                            break;
                        case Direction.South:
                            if (pos == CellComponent.Position.SouthWall)
                            {
                                GD.Print("blocking movement from direction: " + direction);
                                return false;
                            }

                            break;
                        case Direction.East:
                            if (pos == CellComponent.Position.EastWall)
                            {
                                GD.Print("blocking movement from direction: " + direction);
                                return false;
                            }

                            break;
                        case Direction.West:
                            if (pos == CellComponent.Position.WestWall)
                            {
                                GD.Print("blocking movement from direction: " + direction);
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
