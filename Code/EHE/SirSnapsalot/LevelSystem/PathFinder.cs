using Godot;

namespace EHE.SirSnapsalot.LevelSystem
{
    public class PathFinder
    {
        public LevelGrid Grid;

        public PathFinder(LevelGrid grid)
        {
            Grid = grid;
        }

        private AStar2D _aStar;

        public void GenerateNavMap()
        {
            if (_aStar != null)
            {
                _aStar.Clear();
            }
            else
            {
                _aStar = new AStar2D();
            }

            foreach (var cell in Grid.Cells)
            {
                Vector2 position = cell.Key;
                _aStar.AddPoint(cell.Value.GridId, position);
                GD.Print("Adding nav point  at: " + position + " with ID:  " + cell.Value.GridId);
            }

            ConnectNavigationPoints();
        }

        public Vector2[] GetPath(Vector2 start, Vector2 end)
        {
            long startId = _aStar.GetClosestPoint(start);
            long endId = _aStar.GetClosestPoint(end);

            if (startId == -1 || endId == -1)
            {
                GD.Print("No valid start or end point found for pathfinding.");
                return null;
            }

            var path = _aStar.GetPointPath(startId, endId);
            GD.Print("Calculated path from " + start + " to " + end + ":");
            foreach (var point in path)
            {
                GD.Print(point);
            }

            return path;
        }

        private void ConnectNavigationPoints()
        {
            long[] points = _aStar.GetPointIds();
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 pos = _aStar.GetPointPosition(points[i]);
                GridCell cell = GetGridCell(pos);
                if (cell == null)
                {
                    break;
                }

                GD.Print("Checking neighbors for cell at: " + pos + " with ID: " + cell.GridId);

                Vector2 neighborEastPos = new Vector2(pos.X + 1, pos.Y);
                GridCell neighborEast = GetGridCell(neighborEastPos);
                GD.Print("Checking neighbor to the east: " + neighborEastPos);
                if (neighborEast == null)
                {
                    GD.Print("Neighbor to the east is null.");
                }

                Vector2 neighborWestPos = new Vector2(pos.X - 1, pos.Y);
                GridCell neighborWest = GetGridCell(neighborWestPos);
                GD.Print("Checking neighbor to the west: " + neighborWestPos);
                if (neighborWest == null)
                {
                    GD.Print("Neighbor to the west is null.");
                }

                Vector2 neighborNorthPos = new Vector2(pos.X, pos.Y - 1);
                GridCell neighborNorth = GetGridCell(neighborNorthPos);
                GD.Print("Checking neighbor to the north: " + neighborNorthPos);
                if (neighborNorth == null)
                {
                    GD.Print("Neighbor to the north is null.");
                }

                Vector2 neighborSouthPos = new Vector2(pos.X, pos.Y + 1);
                GridCell neighborSouth = GetGridCell(neighborSouthPos);
                GD.Print("Checking neighbor to the south: " + neighborSouthPos);
                if (neighborSouth == null)
                {
                    GD.Print("Neighbor to the south is null.");
                }

                if (
                    neighborEast != null
                    && cell.IsPassableToDirection(GridCell.Direction.East)
                    && neighborEast.IsPassableFromDirection(GridCell.Direction.West)
                )
                {
                    _aStar.ConnectPoints(cell.GridId, neighborEast.GridId, false);
                    GD.Print("Connected points: " + cell.GridId + " to " + neighborEast.GridId);
                }

                if (
                    neighborWest != null
                    && cell.IsPassableToDirection(GridCell.Direction.West)
                    && neighborWest.IsPassableFromDirection(GridCell.Direction.East)
                )
                {
                    _aStar.ConnectPoints(cell.GridId, neighborWest.GridId, false);
                    GD.Print("Connected points: " + cell.GridId + " to " + neighborWest.GridId);
                }

                if (
                    neighborNorth != null
                    && cell.IsPassableToDirection(GridCell.Direction.North)
                    && neighborNorth.IsPassableFromDirection(GridCell.Direction.South)
                )
                {
                    _aStar.ConnectPoints(cell.GridId, neighborNorth.GridId, false);
                    GD.Print("Connected points: " + cell.GridId + " to " + neighborNorth.GridId);
                }

                if (
                    neighborSouth != null
                    && cell.IsPassableToDirection(GridCell.Direction.South)
                    && neighborSouth.IsPassableFromDirection(GridCell.Direction.North)
                )
                {
                    _aStar.ConnectPoints(cell.GridId, neighborSouth.GridId, false);
                    GD.Print("Connected points: " + cell.GridId + " to " + neighborSouth.GridId);
                }
            }
        }

#nullable enable
        private GridCell? GetGridCell(Vector2 position)
        {
            if (Grid.Cells.TryGetValue((Vector2I)position, out GridCell cell))
            {
                return cell;
            }

            return null;
        }
    }
}
