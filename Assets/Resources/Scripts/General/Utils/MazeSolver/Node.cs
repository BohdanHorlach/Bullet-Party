namespace General.Utils.MazeSolver
{
    internal class Node
    {
        public WayPoint2D Point;
        public Node PrevPoint;
        public float G;
        public float F;

        public Node(WayPoint2D point, Node prevPoint, float g, float f)
        {
            Point = point;
            PrevPoint = prevPoint;
            G = g;
            F = f;
        }
    }
}