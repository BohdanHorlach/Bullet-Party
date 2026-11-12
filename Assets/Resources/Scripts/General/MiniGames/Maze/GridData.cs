using UnityEngine;

namespace General.MiniGames.Maze
{
    public struct GridData
    {
        public CellType[,] Grid;
        public Vector2Int Center;
        public int Width;
        public int Height;
    }
}
