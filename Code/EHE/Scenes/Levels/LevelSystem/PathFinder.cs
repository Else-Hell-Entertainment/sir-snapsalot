using Godot;

namespace EHE.LevelSystem
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

                Vector2 nbrEastPos = new Vector2(pos.X + 1, pos.Y);
                GridCell nbrEast = GetGridCell(nbrEastPos);
                GD.Print("Checking neighbor to the east: " + nbrEastPos);
                if (nbrEast == null)
                {
                    GD.Print("Neighbor to the east is null.");
                }

                Vector2 nbrWestPos = new Vector2(pos.X - 1, pos.Y);
                GridCell nbrWest = GetGridCell(nbrWestPos);
                GD.Print("Checking neighbor to the west: " + nbrWestPos);
                if (nbrWest == null)
                {
                    GD.Print("Neighbor to the west is null.");
                }

                Vector2 nbrNorthPos = new Vector2(pos.X, pos.Y - 1);
                GridCell nbrNorth = GetGridCell(nbrNorthPos);
                GD.Print("Checking neighbor to the north: " + nbrNorthPos);
                if (nbrNorth == null)
                {
                    GD.Print("Neighbor to the north is null.");
                }

                Vector2 nbrSouthPos = new Vector2(pos.X, pos.Y + 1);
                GridCell nbrSouth = GetGridCell(nbrSouthPos);
                GD.Print("Checking neighbor to the south: " + nbrSouthPos);
                if (nbrSouth == null)
                {
                    GD.Print("Neighbor to the south is null.");
                }

                if (
                    nbrEast != null
                    && cell.IsPassableToDirection(GridCell.Direction.East)
                    && nbrEast.IsPassableFromDirection(GridCell.Direction.West)
                )
                {
                    _aStar.ConnectPoints(cell.GridId, nbrEast.GridId, false);
                    GD.Print("Connected points: " + cell.GridId + " to " + nbrEast.GridId);
                }

                if (
                    nbrWest != null
                    && cell.IsPassableToDirection(GridCell.Direction.West)
                    && nbrWest.IsPassableFromDirection(GridCell.Direction.East)
                )
                {
                    _aStar.ConnectPoints(cell.GridId, nbrWest.GridId, false);
                    GD.Print("Connected points: " + cell.GridId + " to " + nbrWest.GridId);
                }

                if (
                    nbrNorth != null
                    && cell.IsPassableToDirection(GridCell.Direction.North)
                    && nbrNorth.IsPassableFromDirection(GridCell.Direction.South)
                )
                {
                    _aStar.ConnectPoints(cell.GridId, nbrNorth.GridId, false);
                    GD.Print("Connected points: " + cell.GridId + " to " + nbrNorth.GridId);
                }

                if (
                    nbrSouth != null
                    && cell.IsPassableToDirection(GridCell.Direction.South)
                    && nbrSouth.IsPassableFromDirection(GridCell.Direction.North)
                )
                {
                    _aStar.ConnectPoints(cell.GridId, nbrSouth.GridId, false);
                    GD.Print("Connected points: " + cell.GridId + " to " + nbrSouth.GridId);
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
