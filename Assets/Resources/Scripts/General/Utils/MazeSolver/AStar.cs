using System.Collections.Generic;
using System.Linq;


namespace General.Utils.MazeSolver
{
    public static class AStar
    {
        private static readonly List<Node> _open = new();
        private static readonly List<Node> _closed = new();

        private static Node GetNodeByMinF()
        {
            return _open.OrderBy(n => n.F).FirstOrDefault();
        }

        private static void AddNextPoint(
            List<WayPoint2D> wayPoints,
            Node prevPoint,
            WayPoint2D endPoint,
            HeuristicFunction2D heuristic,
            bool canCarve,
            float penaltyForCarve
        )
        {
            foreach (WayPoint2D point in wayPoints)
            {
                if (_closed.Any(n => n.Point == point))
                    continue;

                if (point.IsWall && canCarve == false)
                    continue;

                float cost = point.IsWall ? (canCarve ? penaltyForCarve : float.MaxValue) : 1f;

                float g = prevPoint.G + cost + heuristic(prevPoint.Point.Position, point.Position);
                float f = g + heuristic(point.Position, endPoint.Position);

                Node openNode = _open.FirstOrDefault(n => n.Point == point);

                if (openNode == null)
                {
                    _open.Add(new Node(point, prevPoint, g, f));
                }
                else
                {
                    if (g >= openNode.G)
                        continue;

                    openNode.PrevPoint = prevPoint;
                    openNode.G = g;
                    openNode.F = f;
                }
            }
        }

        private static List<WayPoint2D> GetPathByNode(Node node)
        {
            var path = new List<WayPoint2D> { node.Point };

            Node current = node.PrevPoint;
            while (current != null)
            {
                path.Insert(0, current.Point);
                current = current.PrevPoint;
            }

            return path;
        }


        public static List<WayPoint2D> FindPath(
            WayPoint2D start,
            WayPoint2D end,
            HeuristicFunction2D heuristic,
            bool canCarve,
            float penaltyForCarve = 40f
        )
        {
            _open.Clear();
            _closed.Clear();

            Node startNode = new Node(
                start,
                null,
                0,
                heuristic(start.Position, end.Position)
            );
            Node lastNode = startNode;

            _open.Add(startNode);

            while (true)
            {
                if (_open.Count == 0)
                    return GetPathByNode(lastNode);

                Node node = GetNodeByMinF();

                _open.Remove(node);

                if (node.Point.Position == end.Position)
                    return GetPathByNode(node);

                AddNextPoint(node.Point.NextPoints, node, end, heuristic, canCarve, penaltyForCarve);
                _closed.Add(node);
                lastNode = node;
            }
        }
    }
}