using UnityEngine;

namespace General.Utils.MazeSolver
{
    public delegate float HeuristicFunction2D(Vector2Int a, Vector2Int b);

    internal static class Heuristics2D
    {
        public static float Manhattan(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        public static float Euclidean(Vector2Int a, Vector2Int b)
        {
            return Vector2Int.Distance(a, b);
        }
    }
}