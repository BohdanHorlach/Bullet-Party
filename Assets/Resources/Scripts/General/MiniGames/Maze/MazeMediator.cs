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
        [SerializeField, Min(0)] private float _penaltyForCarve = 40f;

        [Networked, OnChangedRender(nameof(OnMazeSeedChanged))]
        private int MazeSeed { get; set; }

        private readonly MazeGenerator _mazeGenerator = new();


        public override void Spawned()
        {
            base.Spawned();

            if (HasStateAuthority)
                MazeSeed = PickSeed();
            else if (MazeSeed != 0)
                RegenerateAndFill();
        }


        public void GenerateMaze()
        {
            if (!HasStateAuthority)
                return;

            MazeSeed = PickSeed();
        }


        private int PickSeed()
        {
            return unchecked((int)Runner.Tick * 486187739 + _width * 846463 + _height);
        }


        private void OnMazeSeedChanged()
        {
            if (MazeSeed == 0)
                return;

            RegenerateAndFill();
        }


        private void RegenerateAndFill()
        {
            GridData maze = _mazeGenerator.Generate(
                _width,
                _height,
                MazeSeed,
                _penaltyForCarve
            );

            _mazeFiller.FillMap(maze, _roadMap, _wallMap);
        }
    }
}
