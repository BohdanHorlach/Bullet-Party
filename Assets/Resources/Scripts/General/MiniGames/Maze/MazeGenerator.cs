using General.Utils.MazeSolver;
using System.Collections.Generic;
using UnityEngine;


namespace General.MiniGames.Maze
{
    public class MazeGenerator
    {
        private static readonly Vector2Int[] DIRECTIONS =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.right,
            Vector2Int.left
        };

        private GridData _buffer;


        private void Init(int w, int h)
        {
            _buffer.Width = w;
            _buffer.Height = h;
            _buffer.Center = new Vector2Int(w / 2, h / 2);
            _buffer.Grid = new CellType[w, h];
        }

        private void FillWalls()
        {
            for (int x = 0; x < _buffer.Width; x++)
                for (int y = 0; y < _buffer.Height; y++)
                    _buffer.Grid[x, y] = CellType.Wall;
        }

        private bool IsInside(Vector2Int p) 
        { 
            return p.x > 0 
                && p.y > 0 
                && p.x < _buffer.Width - 1 
                && p.y < _buffer.Height - 1; 
        }


        private int GenerateSeed()
        {
            var rng = new System.Random();
            byte[] bytes = new byte[4];
            rng.NextBytes(bytes);

            int seed = System.BitConverter.ToInt32(bytes, 0);

            return seed;
        }


        private Vector2Int ChooseActiveCell(List<Vector2Int> active)
        {
            float r = Random.value;

            if (r < 0.20f)
                return active[0];
            if (r < 0.70f)
                return active[^1];

            return active[Random.Range(0, active.Count)];
        }


        private List<Vector2Int> GetUnvisitedNeighbors(
            Vector2Int cell,
            HashSet<Vector2Int> visited,
            int width,
            int height
        )
        {
            var result = new List<Vector2Int>();

            foreach (var dir in DIRECTIONS)
            {
                Vector2Int next = cell + dir * 2;

                if (next.x <= 0 || next.y <= 0 ||
                    next.x >= width - 1 || next.y >= height - 1)
                    continue;

                if (!visited.Contains(next))
                    result.Add(next);
            }

            return result;
        }

        private void GenerateMaze(Vector2Int start)
        {
            var active = new List<Vector2Int>();
            var visited = new HashSet<Vector2Int>();

            _buffer.Grid[start.x, start.y] = CellType.Corridor;
            active.Add(start);
            visited.Add(start);

            while (active.Count > 0)
            {
                Vector2Int current = ChooseActiveCell(active);

                var neighbors = GetUnvisitedNeighbors(current, visited, _buffer.Width, _buffer.Height);

                if (neighbors.Count == 0)
                {
                    active.Remove(current);
                    continue;
                }

                Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];
                Vector2Int wall = (current + next) / 2;

                _buffer.Grid[wall.x, wall.y] = CellType.Corridor;
                _buffer.Grid[next.x, next.y] = CellType.Corridor;

                visited.Add(next);
                active.Add(next);
            }
        }


        private void MakeOdd(ref Vector2Int p)
        {
            if (p.x % 2 == 0) p.x += 1;
            if (p.y % 2 == 0) p.y += 1;
        }


        private void CarvePath(List<WayPoint2D> path, Vector2Int direction)
        {
            foreach (var p in path)
            {
                if(IsInside(p.Position))
                    _buffer.Grid[p.Position.x, p.Position.y] = CellType.Corridor;
            }

            WayPoint2D preLastPoint = path[^2];
            WayPoint2D lastPoint = path[^1];

            Vector2Int enterPosition = lastPoint.Position + direction;
            _buffer.Grid[enterPosition.x, enterPosition.y] = CellType.Corridor;
        }


        private Vector2Int[] BuildTargets(Vector2Int center, Vector2Int[] directions)
        {
            int radius = _buffer.Width / 2 - 1;
            Vector2Int[] targets = new Vector2Int[directions.Length];

            for(int i = 0; i < directions.Length; i++)
            {
                Vector2Int target = center + directions[i] * radius;
                targets[i] = target;
            }

            for (int i = 0; i < targets.Length; i++)
                MakeOdd(ref targets[i]);

            return targets;
        }


        private WayPoint2D[,] BuildGraph()
        {
            int width = _buffer.Width;
            int height = _buffer.Height;

            var graph = new WayPoint2D[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bool isWall = _buffer.Grid[x, y] == CellType.Wall;
                    graph[x, y] = new WayPoint2D(new Vector2Int(x, y), isWall);
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    WayPoint2D wp = graph[x, y];
                    if (wp == null) continue;

                    foreach (Vector2Int dir in DIRECTIONS)
                    {
                        Vector2Int n = wp.Position + dir;
                        if (IsInside(n) && graph[n.x, n.y] != null)
                            wp.NextPoints.Add(graph[n.x, n.y]);
                    }
                }
            }

            return graph;
        }


        private void BuildPaths(float penaltyForCarve)
        {
            Vector2Int center = _buffer.Center;

            Vector2Int[] targets = BuildTargets(center, DIRECTIONS);
            WayPoint2D[,] graph = BuildGraph();

            for(int i = 0; i < targets.Length; i++)
            {
                Vector2Int target = targets[i];
                Vector2Int direction = DIRECTIONS[i];

                List<WayPoint2D> path = AStar.FindPath(
                    graph[center.x, center.y],
                    graph[target.x, target.y],
                    Heuristics2D.Euclidean,
                    true,
                    penaltyForCarve
                );

                CarvePath(path, direction);
            }
        }


        public GridData Generate(
            int width,
            int height,
            float forwardWeight,
            float randomMaxWeight,
            float noiseWeight,
            float noiseScale,
            float penaltyForCarve
        )
        {
            int seed = GenerateSeed();
            float seedOffset = (seed % 1000) / 1000f;
            Random.InitState(seed);

            Init(width, height);
            FillWalls();

            Vector2Int center = _buffer.Center;

            _buffer.Grid[center.x, center.y] = CellType.Corridor;

            GenerateMaze(new Vector2Int(1,1));
            BuildPaths(penaltyForCarve);

            return _buffer;
        }
    }
}