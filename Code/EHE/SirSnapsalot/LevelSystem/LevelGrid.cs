// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>
//         Miska Rihu <miska.rihu@tuni.fi>

using System;
using System.Collections.Generic;
using EHE.SirSnapsalot.Config;
using Godot;

namespace EHE.SirSnapsalot.LevelSystem
{
    public partial class LevelGrid : Node3D
    {
        private int _width = 0;
        private int _height = 0;
        private int _gridSize = SystemConfig.GridScale;

        private Plane _zeroPlane = new Plane(Vector3.Up, 0);

        private CellComponent.Position _ghostPosition = CellComponent.Position.None;

        private CellComponent _ghostComponent;
        private GridCell _rootCell; // Cell at position (0,0) to hold ghost components before placement.

        private List<MeshInstance3D> _navigationPath = new List<MeshInstance3D>();
        private PathFinder _pathFinder; // TODO: Move this field to the Level class.

        private Node3D _ghostInstance;

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

        public Godot.Collections.Dictionary<Vector2I, GridCell> Cells;

        #region Godot API

        public override void _Ready()
        {
            Initialize();
            PopulateCellDictionary();
            _pathFinder = new PathFinder(this);
        }

        public override void _PhysicsProcess(double delta)
        {
            UpdateGhost();
#if DEBUG
            DebugDraw(delta);
#endif
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event.IsActionPressed("Confirm"))
            {
                PlaceGhostComponent();
            }
        }

        #endregion

        #region Public API

        public void Initialize()
        {
            PopulateCellDictionary();
            _pathFinder = new PathFinder(this);
            CreateGridCell(new Vector2I(0, 0));
            _rootCell = GetGridCell(new Vector2I(0, 0));
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

        /// <summary>
        /// Finds a navigation path between two positions in world space.
        /// </summary>
        /// <param name="startPositionWorld">The path's starting position in world coordinates.</param>
        /// <param name="endPositionWorld">The path's destination in world coordinates.</param>
        /// <returns>
        /// The path as an array of world-space cell positions, or <c>null</c>
        /// if no valid path is found or a path cell is missing from the grid.
        /// </returns>
        /// <seealso cref="GetNavPath(Vector2, Vector2)"/>
        public Vector3[] GetNavPath(Vector3 startPositionWorld, Vector3 endPositionWorld)
        {
            return GetNavPath(
                startPositionGrid: GetGridCoordinatesFromPosition(startPositionWorld),
                endPositionGrid: GetGridCoordinatesFromPosition(endPositionWorld)
            );
        }

        /// <summary>
        /// Finds a navigation path between two grid positions.
        /// </summary>
        /// <param name="startPositionGrid">The path's starting position in grid coordinates.</param>
        /// <param name="endPositionGrid">The path's destination in grid coordinates.</param>
        /// <returns>
        /// The path as an array of world-space cell positions, or <c>null</c>
        /// if no valid path is found or a path cell is missing from the grid.
        /// </returns>
        /// /// <seealso cref="GetNavPath(Vector3, Vector3)"/>
        public Vector3[] GetNavPath(Vector2 startPositionGrid, Vector2 endPositionGrid)
        {
            var path = _pathFinder.GetPath(startPositionGrid, endPositionGrid);
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
            cell.GridCoordinates = gridCoordinates;
            // var sceneOwner = GetTree().CurrentScene;
            // if (sceneOwner != null)
            // {
            //     cell.Owner = sceneOwner;
            // }
            cell.Owner = Owner;

            Cells[gridCoordinates] = cell;
        }


        /// <summary>
        /// Returns all adjacent grid cells around the given world position.
        /// </summary>
        ///
        /// <param name="globalPosition">
        /// The world position to inspect. It is converted to grid coordinates
        /// using the current grid scale.
        /// </param>
        /// <param name="includeDiagonalNeighbors">
        /// When <c>true</c>, diagonal neighbors are included in the result
        /// alongside orthogonal neighbors; otherwise only orthogonal neighbors
        /// are returned.
        /// </param>
        ///
        /// <returns>
        /// A list of neighboring <see cref="GridCell"/> instances that exist
        /// in the grid. Returns an empty list
        /// if no neighbors are found.
        /// </returns>
        public IList<GridCell> GetNeighbors(Vector3 globalPosition, bool includeDiagonalNeighbors = false)
        {
            // TODO: Is is a bug if we try to find neighboring cells for a position that doesn't have a cell?
            Vector2I gridPosition = GetGridCoordinatesFromPosition(globalPosition);
            List<GridCell> neighbors = new List<GridCell>();

            for (int offsetX = -1; offsetX <= 1; offsetX++)
            {
                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    if (offsetX == 0 && offsetY == 0)
                    {
                        // Skip the position whose neighbors are being searched.
                        continue;
                    }

                    if (!includeDiagonalNeighbors && Mathf.Abs(offsetX) == Mathf.Abs(offsetY))
                    {
                        // Skip diagonal neighbors.
                        continue;
                    }

                    int neighborX = gridPosition.X + offsetX;
                    int neighborY = gridPosition.Y + offsetY;

                    GridCell neighbor = GetGridCell(new Vector2I(neighborX, neighborY));

                    if (neighbor != null)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Finds the GridCell that is in the given global position.
        /// </summary>
        /// <param name="globalPosition">The world position to check.</param>
        /// <returns>
        /// Reference to the GridCell in the given world position or
        /// <c>null</c> if no GridCell is found in that position.
        /// </returns>
        public GridCell GetGridCell(Vector3 globalPosition)
        {
            Vector3 localPosition = ToLocal(globalPosition);

            // Check if the localPosition is within the grid bounds.
            // TODO: Should this be done in GetGridCoordinatedFromPosition instead?
            if (localPosition.X < 0 || localPosition.Z < 0)
            {
                return null;
            }

            Vector2I gridCoordinates = GetGridCoordinatesFromPosition(localPosition);
            return GetGridCell(gridCoordinates);
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
            switch (component.ComponentType)
            {
                case CellComponent.CellComponentType.Wall:
                    // Walls are aligned to the edges of the cell.
                    _ghostPosition = CellComponent.Position.NorthWall;
                    break;
                default:
                    // Everything else is aligned to center of the cell.
                    _ghostPosition = CellComponent.Position.Floor;
                    break;
            }

            _ghostInstance = (Node3D)component.GetSceneInstance();
            AddChild(_ghostInstance);
        }

        public void CalculateNavigation()
        {
            _pathFinder.GenerateNavMap();
        }

        // TODO: Maybe move this method to PathFinder?
        /// <summary>
        /// Draws the navigation path between two world-space positions on the
        /// grid, replacing any previously drawn path.
        /// </summary>
        /// <param name="startPosition">The path's starting position in world coordinates.</param>
        /// <param name="endPosition">The path's destination in world coordinates.</param>
        /// <seealso cref="DrawNavigationPath(Vector2, Vector2)"/>
        public void DrawNavigationPath(Vector3 startPosition, Vector3 endPosition)
        {
            DrawNavigationPath(
                startPosition: GetGridCoordinatesFromPosition(startPosition),
                endPosition: GetGridCoordinatesFromPosition(endPosition)
            );
        }

        // TODO: Maybe move this method to PathFinder?
        /// <summary>
        /// Draws the navigation path between two grid positions, replacing any
        /// previously drawn path.
        /// </summary>
        /// <param name="startPosition">The path's starting position in grid coordinates.</param>
        /// <param name="endPosition">The path's destination in grid coordinates.</param>
        /// <seealso cref="DrawNavigationPath(Vector3, Vector3)"/>
        public void DrawNavigationPath(Vector2 startPosition, Vector2 endPosition)
        {
            foreach (var mesh in _navigationPath)
            {
                mesh.QueueFree();
            }

            _navigationPath.Clear();
            var path = _pathFinder.GetPath(startPosition, endPosition);
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

        #endregion

        #region Private API

        private void PopulateCellDictionary()
        {
            if (Cells == null)
            {
                Cells = new Godot.Collections.Dictionary<Vector2I, GridCell>();
            }
            else
            {
                Cells.Clear();
            }

            foreach (var child in GetChildren())
            {
                if (child is GridCell cell)
                {
                    Cells[new Vector2I(cell.GridCoordinates.X, cell.GridCoordinates.Y)] = cell;
                }
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
                    _rootCell.SetComponentRotation(_ghostInstance, _ghostPosition);
                    cell.SetComponentPosition(_ghostInstance, _ghostPosition);
                }
            }
        }

        // TODO: Refactor IsPlacementValid logic to make expansion easier.
        // For example, add exported RequiresFloor property to CellComponent and pass the cell component here as param.
        // -> If (requires floor && no floor): return false
        // -> Else: create cell & return true
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

                foreach (var comp in cell.Components)
                {
                    if (
                        comp.Position == CellComponent.Position.Floor
                        && comp.Component.ComponentType == CellComponent.CellComponentType.Floor
                    )
                    {
                        return false; // Cannot place a floor where one already exists.
                    }
                }

                return true; // Can place a floor if no floor exists at that position.
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

            if (_ghostComponent.ComponentType == CellComponent.CellComponentType.Trap)
            {
                if (cell != null)
                {
                    return true;
                }
            }

            if (_ghostComponent.ComponentType == CellComponent.CellComponentType.Navigation)
            {
                if (cell != null)
                {
                    return true;
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

        #endregion

        #region Debug Stuff

        [ExportCategory("Debug Draw")]
        [Export]
        private bool _useDebugDraw = false;

        [Export]
        private double _debugDrawRefreshInterval = 0.5;

        // Common
        private double _debugDrawRefreshCounter = 0.0;
        private bool _wasUsed = false;

        // DebugDrawGridCoordinates
        private Node _debugDrawGridCoordinatesRoot;

#if DEBUG
        // Calls other debug draw methods on intervals.
        private void DebugDraw(double delta)
        {
            if (!_useDebugDraw)
            {
                if (_wasUsed)
                {
                    // Clears debug stuff from scene when debug draw is
                    // disabled during runtime.
                    DebugDrawClear();
                }

                _wasUsed = false;
                return;
            }

            if (_debugDrawRefreshCounter == 0)
            {
                // Put other debug draw method calls here.
                DebugDrawGridCoordinates();
            }

            _debugDrawRefreshCounter += delta;

            if (_debugDrawRefreshCounter >= _debugDrawRefreshInterval)
            {
                _debugDrawRefreshCounter = 0;
            }

            _wasUsed = true;
        }

        // Removes debug draw objects from the scene.
        // Add necessary cleaning stuff here.
        private void DebugDrawClear()
        {
            // DebugDrawGridCoordinates
            _debugDrawGridCoordinatesRoot.Free();
            _debugDrawGridCoordinatesRoot = null;
        }

        // Places text meshes in the scene to show cell coordinates.
        private void DebugDrawGridCoordinates()
        {
            if (!_useDebugDraw)
            {
                return;
            }

            if (_debugDrawGridCoordinatesRoot == null)
            {
                _debugDrawGridCoordinatesRoot = new Node();
                AddChild(_debugDrawGridCoordinatesRoot);
            }

            foreach (var child in _debugDrawGridCoordinatesRoot.GetChildren())
            {
                child.QueueFree();
            }

            foreach (var cell in Cells.Values)
            {
                float x = cell.GlobalPosition.X;
                float z = cell.GlobalPosition.Z;
                float y = 0;

                MeshInstance3D mesh = new MeshInstance3D()
                {
                    Mesh = new TextMesh()
                    {
                        Text = $"{cell.GridCoordinates}",
                        FontSize = 48
                    },
                    RotationDegrees = new Vector3(-90,0,0)
                };

                _debugDrawGridCoordinatesRoot.AddChild(mesh);
                mesh.GlobalPosition = new Vector3(x, y, z);
            }
        }

#endif
        #endregion
    }
}
