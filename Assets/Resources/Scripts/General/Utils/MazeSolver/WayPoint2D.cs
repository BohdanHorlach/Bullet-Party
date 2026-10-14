using System.Collections.Generic;
using UnityEngine;


namespace General.Utils.MazeSolver
{
    public class WayPoint2D
    {
        public List<WayPoint2D> NextPoints = new();
        public Vector2Int Position;
        public bool IsWall;

        public WayPoint2D(Vector2Int position, bool isWall)
        {
            Position = position;
            IsWall = isWall;
        }
    }
}