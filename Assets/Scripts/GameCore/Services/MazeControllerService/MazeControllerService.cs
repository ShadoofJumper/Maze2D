using GameCore.Controllers.MazeBuilder;
using GameCore.Data;
using GameCore.Services.SceneControllerService;
using UnityEngine;

namespace GameCore.Services.MazeControllerService
{
    public class MazeControllerService: IMazeControllerService
    {
        private ISceneControllerService _sceneControllerService;
        
        private MazeCell[,] _currentMaze;
        private Vector3 _mazeCenter;

        private const float MAZE_CELL_OFFSET = 0.5f;
        private const float MAZE_CELL_SIZE = 1.0f;
        private static readonly Vector3 MAZE_ORIGIN = new Vector3(0, 0,0);
        
        public float MazeCellSize => MAZE_CELL_SIZE;
        public Vector3 MazeCenter => _mazeCenter;

        public MazeControllerService(ISceneControllerService sceneControllerService)
        {
            _sceneControllerService = sceneControllerService;
        }

        private void BuildMazeView(MazeCell[,] mazeData)
        {
            _sceneControllerService.ClearMazeMap();
            foreach (var mazeCell in mazeData)
            {
                _sceneControllerService.CreateMazeCell(mazeCell);
            }
        }
        
        public void CreateTheMaze(MazeConfig mazeConfig)
        {
            MazeGenerator mazeGenerator = new MazeGenerator(mazeConfig.Width, mazeConfig.Height, mazeConfig.NumberOfExits);
            _currentMaze = mazeGenerator.Generate();
            BuildMazeView(_currentMaze);
            _mazeCenter = new Vector3(mazeGenerator.StartCell.x, mazeGenerator.StartCell.y, 0);
        }

        public bool IsCellFree(Vector2 cellPos)
        {
            if (!IsInsideMap(cellPos))
                return false;

            return _currentMaze[(int)cellPos.x, (int)cellPos.y].Type == MazeCellType.Floor
                   || _currentMaze[(int)cellPos.x, (int)cellPos.y].Type == MazeCellType.Exit;
        }

        public bool IsCellIsExit(Vector2 cellPos)
        {
            if (!IsInsideMap(cellPos))
                return false;

            return _currentMaze[(int)cellPos.x, (int)cellPos.y].Type == MazeCellType.Exit;
        }

        private bool IsInsideMap(Vector2 cellPos)
        {
            return cellPos.x >= 0 && cellPos.y >= 0 &&
                   cellPos.x < _currentMaze.GetLength(0) && cellPos.y < _currentMaze.GetLength(1);
        }
        
        public Vector3 MazeCoordToWorld(Vector2 cellPos)
        {
            return new Vector3(cellPos.x + MAZE_CELL_OFFSET, cellPos.y + MAZE_CELL_OFFSET, 0);
        }

        public Vector2 WorldPosToMaze(Vector3 worldPos)
        {
            // Convert world position to local maze space
            Vector3 localPos = worldPos - MAZE_ORIGIN;

            int x = Mathf.FloorToInt(localPos.x / MAZE_CELL_SIZE);
            int y = Mathf.FloorToInt(localPos.y / MAZE_CELL_SIZE);

            return new Vector2Int(x, y);
        }
    }
}