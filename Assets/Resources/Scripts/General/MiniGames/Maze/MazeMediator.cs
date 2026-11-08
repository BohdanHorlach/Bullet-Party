using Fusion;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace General.MiniGames.Maze
{
    public class MazeMediator : NetworkBehaviour
    {
        [SerializeField] private MazeFiller _mazeFiller;
        [SerializeField] private Tilemap _roadMap;
        [SerializeField] private Tilemap _wallMap;
        [SerializeField] private int _width = 51;
        [SerializeField] private int _height = 51;
        [SerializeField, Range(0, 1)] float _forwardWeight = 0.3f;
        [SerializeField, Range(0, 2)] private float _randomMaxWeight = 1.5f;
        [SerializeField, Range(0, 1)] private float _noiseWeight = 0.5f;
        [SerializeField, Range(0, 1)] private float _noiseScale = 1f;
        [SerializeField, Min(0)] private float _penaltyForCarve = 40f;


        private MazeGenerator _mazeGenerator = new MazeGenerator();


        public override void Spawned()
        {
            base.Spawned();
            GenerateMaze();
        }


        public void GenerateMaze()
        {
            if (!Runner.IsServer)
                return;

            GridData maze = _mazeGenerator.Generate(
                _width, 
                _height, 
                _forwardWeight, 
                _randomMaxWeight, 
                _noiseWeight, 
                _noiseScale,
                _penaltyForCarve
            );

            _mazeFiller.FillMap(maze, _roadMap, _wallMap);
        }
    }
}