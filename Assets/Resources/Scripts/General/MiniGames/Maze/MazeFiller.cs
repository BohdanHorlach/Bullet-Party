using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace General.MiniGames.Maze
{
    public class MazeFiller : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;
        [Header("Tiles")]
        [SerializeField] private TileBase _wallTile;
        [SerializeField] private TileBase _corridorTile;
        [SerializeField] private TileBase _finishTile;
        [SerializeField] private int _centerRadius = 1;


        private void SetTile(int x, int y, TileBase tile, GridData data)
        {
            int offsetX = data.Width / 2;
            int offsetY = data.Height / 2;

            Vector3Int tilePos = new(
                x - offsetX,
                y - offsetY,
                0
            );

            _tilemap.SetTile(tilePos, tile);
        }


        private List<Vector2Int> GetPointsInSquare(Vector2Int center, int radius)
        {
            List<Vector2Int> result = new List<Vector2Int>();

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    result.Add(new Vector2Int(
                        center.x + x,
                        center.y + y
                    ));
                }
            }

            return result;
        }


        private void FillArea(Vector2Int center, int radius, GridData gridData)
        {
            List<Vector2Int> points = GetPointsInSquare(center, radius);

            foreach (Vector2Int point in points) 
                SetTile(point.x, point.y, _finishTile, gridData);
        }


        public void FillMap(GridData gridData)
        {
            _tilemap.ClearAllTiles();

            for (int x = 0; x < gridData.Width; x++)
            {
                for (int y = 0; y < gridData.Height; y++)
                {
                    var cellType = gridData.Grid[x, y];

                    if (cellType == CellType.Corridor)
                        SetTile(x, y, _corridorTile, gridData);
                    else
                        SetTile(x, y, _wallTile, gridData);
                }
            }

            Vector2Int center = gridData.Center;
            FillArea(center, _centerRadius, gridData);
        }
    }
}