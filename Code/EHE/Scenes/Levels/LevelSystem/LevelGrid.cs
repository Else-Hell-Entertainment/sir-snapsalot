using System;
using System.Collections.Generic;
using EHE.Global.Config;
using Godot;

namespace EHE.LevelSystem
{
    public partial class LevelGrid : Node3D
    {
        private int _width = 0;
        private int _height = 0;
        private int _gridSize = SystemConfig.GridScale;

        private GridCell _rootCell;

        private Plane _zeroPlane = new Plane(Vector3.Up, 0);

        private CellComponent.Position _ghostPosition = CellComponent.Position.NONE;

        private CellComponent _ghostComponent;

        private List<MeshInstance3D> _navigationPath = new List<MeshInstance3D>();
        private PathFinder _pathFinder;

        public CellComponent GhostComponent
        {
            get => _ghostComponent;
            set
            {
                if (value != null)
                {
                    if (_ghostInstance != null)
                    {
                        _ghostInstance.QueueFree();
                        _ghostInstance = null;
                    }

                    _ghostComponent = value;
                    CreateGhostComponent(_ghostComponent);
                }
                else
                {
                    _ghostComponent = null;
                    _ghostInstance?.QueueFree();
                    _ghostInstance = null;
                }
            }
        }

        private Node3D _ghostInstance;

        public Godot.Collections.Dictionary<Vector2I, GridCell> Cells = new();

        public override void _Ready()
        {
            RebuildCells();
        }

        public override void _PhysicsProcess(double delta)
        {
            UpdateGhost();

            if (Input.IsActionJustPressed("Confirm"))
            {
                PlaceGhostComponent();
            }
        }

        private void RebuildCells()
        {
            Cells.Clear();

            foreach (var child in GetChildren())
            {
                if (child is GridCell cell && cell.Name.ToString().StartsWith("Cell_"))
                {
                    var parts = cell.Name.ToString().Split('_');

                    if (parts.Length == 3 && int.TryParse(parts[1], out var x) && int.TryParse(parts[2], out var y))
                    {
                        Cells[new Vector2I(x, y)] = cell;
                    }
                }
            }
        }

        public void Initialize(CellComponent initialFloorPiece)
        {
            Vector2I gridPos = new Vector2I(0, 0);
            CreateGridCell(gridPos);
            _rootCell = GetGridCell(gridPos);
            _rootCell.AddComponent(CellComponent.Position.Floor, initialFloorPiece);
            _rootCell.GenerateCellCellComponents();

            _pathFinder = new PathFinder(this);
        }

        public void RotateGhostComponent(bool clockwise)
        {
            if (_ghostInstance != null)
            {
                SetNextRotation(clockwise);
                _rootCell.SetComponentRotation(_ghostInstance, _ghostPosition);
            }
        }

        public void RemoveCell()
        {
            var mousePos = GetMouseOnPlane();
            Vector2I gridCoordinates = GetGridCoordinatesFromPosition(mousePos);
            var cell = GetGridCell(gridCoordinates);
            if (cell != null)
            {
                cell.ClearCellComponents();
                Cells.Remove(gridCoordinates);
                cell.QueueFree();
                GD.Print("Removed cell at: " + gridCoordinates);
            }
            else
            {
                GD.Print("No cell found at: " + gridCoordinates);
            }
        }

        private void SetNextRotation(bool clockwise)
        {
            switch (_ghostPosition)
            {
                case CellComponent.Position.Floor:
                    break;
                case CellComponent.Position.NorthWall:
                    _ghostPosition = clockwise ? CellComponent.Position.EastWall : CellComponent.Position.WestWall;

                    break;
                case CellComponent.Position.EastWall:
                    _ghostPosition = clockwise ? CellComponent.Position.SouthWall : CellComponent.Position.NorthWall;

                    break;
                case CellComponent.Position.SouthWall:
                    _ghostPosition = clockwise ? CellComponent.Position.WestWall : CellComponent.Position.EastWall;

                    break;
                case CellComponent.Position.WestWall:
                    _ghostPosition = clockwise ? CellComponent.Position.NorthWall : CellComponent.Position.SouthWall;

                    break;
            }
        }

        private void UpdateGhost()
        {
            if (_ghostInstance != null)
            {
                var mousePos = GetMouseOnPlane();
                Vector2I gridCoordinates = GetGridCoordinatesFromPosition(mousePos);
                _ghostInstance.Position = new Vector3(
                    gridCoordinates.X * _gridSize + (float)_gridSize / 2,
                    0,
                    gridCoordinates.Y * _gridSize + (float)_gridSize / 2
                );

                var cell = GetGridCell(gridCoordinates);
                if (cell != null)
                {
                    cell.SetComponentRotation(_ghostInstance, _ghostPosition);
                    cell.SetComponentPosition(_ghostInstance, _ghostPosition);
                }
            }
        }

        private bool IsPlacementValid(Vector2I gridCoordinates)
        {
            if (gridCoordinates.X < 0 || gridCoordinates.Y < 0)
            {
                return false;
            }

            var cell = GetGridCell(gridCoordinates);

            if (_ghostComponent == null)
            {
                return false;
            }

            // Handle special case of creating a new cell for floor components.
            if (_ghostComponent.ComponentType == CellComponent.CellComponentType.Floor)
            {
                if (cell == null)
                {
                    return true; // Can be placed, but cell needs to be created first.
                }
                else
                {
                    return false; // Can only place one floor component per cell.
                }
            }

            if (_ghostComponent.ComponentType == CellComponent.CellComponentType.Wall)
            {
                if (cell != null)
                {
                    foreach (var comp in cell.Components)
                    {
                        if (
                            comp.Position == _ghostPosition
                            && comp.Component.ComponentType == CellComponent.CellComponentType.Wall
                        )
                        {
                            return false; // Cannot place a wall where one already exists.
                        }
                    }

                    return true; // Can place a wall if no wall exists at that position.
                }
                else
                {
                    return false; // Cannot place a wall without an existing cell.
                }
            }

            if (_ghostComponent.ComponentType == CellComponent.CellComponentType.Prop)
            {
                if (cell != null)
                {
                    return true; // Can place a prop if the cell exists.
                }
            }

            return false;
        }

        private void PlaceGhostComponent()
        {
            var mousePos = GetMouseOnPlane();
            Vector2I gridCoordinates = GetGridCoordinatesFromPosition(mousePos);

            if (IsPlacementValid(gridCoordinates))
            {
                var cell = GetGridCell(gridCoordinates);
                if (cell == null && _ghostComponent.ComponentType == CellComponent.CellComponentType.Floor)
                {
                    GD.Print("Cell not found at: " + gridCoordinates + ", creating new cell.");
                    CreateGridCell(gridCoordinates);
                    cell = GetGridCell(gridCoordinates);
                    cell.AddComponent(CellComponent.Position.Floor, _ghostComponent);
                    cell.SpawnCellComponent(CellComponent.Position.Floor, _ghostComponent);
                }
                else if (cell != null)
                {
                    GD.Print("Placing component at: " + gridCoordinates);
                    cell.AddComponent(_ghostPosition, _ghostComponent);
                    cell.SpawnCellComponent(_ghostPosition, _ghostComponent);
                }
            }
            else
            {
                GD.Print("Invalid placement at: " + gridCoordinates);
            }
        }

        public Vector3[] GetNavPath()
        {
            var path = _pathFinder.GetPath(new Vector2(0, 0), new Vector2(5, 5));
            if (path == null || path.Length < 2)
            {
                GD.Print("No valid path found.");
                return null;
            }

            Vector3[] navPath = new Vector3[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                Vector2I gridPos = (Vector2I)path[i];
                var cell = GetGridCell(gridPos);
                if (cell != null)
                {
                    navPath[i] = cell.GlobalPosition;
                }
                else
                {
                    GD.PrintErr("Cell not found at: " + gridPos);
                    return null;
                }
            }

            return navPath;
        }

        public void CreateGridCell(Vector2I gridCoordinates)
        {
            GridCell cell = new GridCell()
            {
                Position = new Vector3(
                    gridCoordinates.X * _gridSize + (float)_gridSize / 2,
                    0,
                    gridCoordinates.Y * _gridSize + (float)_gridSize / 2
                ),
            };

            if (gridCoordinates.X >= _width)
            {
                _width = gridCoordinates.X + 1;
            }

            if (gridCoordinates.Y >= _height)
            {
                _height = gridCoordinates.Y + 1;
            }

            AddChild(cell);
            cell.Name = $"Cell_{gridCoordinates.X}_{gridCoordinates.Y}";
            var sceneOwner = GetTree().CurrentScene;
            if (sceneOwner != null)
            {
                cell.Owner = sceneOwner;
            }

            GD.Print("Cell owner: " + cell.Owner.Name);
            Cells[gridCoordinates] = cell;
        }

        public GridCell GetGridCell(Vector2I gridCoordinates)
        {
            if (Cells.ContainsKey(gridCoordinates))
            {
                return Cells[gridCoordinates];
            }

            return null;
        }

        public void CreateGhostComponent(CellComponent component)
        {
            if (component.ComponentType == CellComponent.CellComponentType.Floor)
            {
                _ghostPosition = CellComponent.Position.Floor;
            }
            else if (component.ComponentType == CellComponent.CellComponentType.Wall)
            {
                _ghostPosition = CellComponent.Position.NorthWall;
            }

            _ghostInstance = (Node3D)component.GetSceneInstance();
            AddChild(_ghostInstance);
        }

        public void CalculateNavigation()
        {
            _pathFinder.GenerateNavMap();
        }

        public void DrawNavigationPath()
        {
            foreach (var mesh in _navigationPath)
            {
                mesh.QueueFree();
            }

            _navigationPath.Clear();
            var path = _pathFinder.GetPath(new Vector2(0, 0), new Vector2(5, 5));
            if (path == null || path.Length < 2)
            {
                GD.Print("No valid path found.");
                return;
            }

            for (int i = 0; i < path.Length - 1; i++)
            {
                Vector2I start = (Vector2I)path[i];
                Vector2I end = (Vector2I)path[i + 1];
                var startCell = GetGridCell(start);
                var endCell = GetGridCell(end);
                if (startCell != null && endCell != null)
                {
                    Vector3 startPos = startCell.GlobalPosition;
                    Vector3 endPos = endCell.GlobalPosition;
                    var lineSegment = CreateLineSegment(startPos, endPos, 0.1f);
                    _navigationPath.Add(lineSegment);
                }
            }
        }

        private MeshInstance3D CreateLineSegment(Vector3 from, Vector3 to, float thickness = 0.05f)
        {
            var meshInstance = new MeshInstance3D();
            var boxMesh = new BoxMesh();
            AddChild(meshInstance);

            float length = from.DistanceTo(to);
            boxMesh.Size = new Vector3(thickness, thickness, length);
            meshInstance.Mesh = boxMesh;

            Vector3 midpoint = (from + to) / 2f;
            meshInstance.GlobalPosition = midpoint;
            meshInstance.LookAtFromPosition(midpoint, to, Vector3.Up);

            return meshInstance;
        }

        private Vector3 GetMouseOnPlane()
        {
            var mousePosition = GetViewport().GetMousePosition();
            var camera = GetViewport().GetCamera3D();
            var from = camera.ProjectRayOrigin(mousePosition);
            var to = from + camera.ProjectRayNormal(mousePosition) * 1000;

            var intersection = _zeroPlane.IntersectsSegment(from, to);
            if ((intersection is Vector3 intersectionPoint))
            {
                return intersectionPoint;
            }
            return Vector3.Zero;
        }

        private Vector2I GetGridCoordinatesFromPosition(Vector3 position)
        {
            int x = Mathf.FloorToInt(position.X / _gridSize);
            x = Math.Max(0, x);
            int y = Mathf.FloorToInt(position.Z / _gridSize);
            y = Math.Max(0, y);
            Vector2I gridCoordinates = new Vector2I(x, y);

            return gridCoordinates;
        }
    }
}
