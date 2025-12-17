using System;
using System.Collections.Generic;
using UnityEngine;


namespace GameCore.Controllers.MazeBuilder
{
    public class MazeGenerator
    {
        private int _width;
        private int _height;
        private int _exitCount;

        private MazeCell[,] _map;
        private System.Random _random = new System.Random();

        private const int StartX = 1;
        private const int StartY = 1;
        
        public MazeGenerator(int width, int height, int exitCount)
        {
            _width = width;
            _height = height;
            
            //maze must be odd for maze to be correct
            if (_width % 2 == 0)
                _width++;
            
            if (_height % 2 == 0)
                _height++;
            
            _exitCount = Mathf.Max(1, exitCount);
        }

        public MazeCell[,] Generate()
        {
            InitializeMap();
            GenerateMaze();
            PlaceExits();

            return _map;
        }

        #region Initialization

        private void InitializeMap()
        {
            _map = new MazeCell[_width, _height];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _map[x, y] = new MazeCell(x, y, MazeCellType.Wall);
                }
            }
        }

        #endregion

        #region Maze generation (DFS)

        private void GenerateMaze()
        {
            if (!IsInBounds(StartX, StartY))
                throw new Exception("Maze too small for start position (2,2)");

            CarvePath(StartX, StartY);
        }

        private void CarvePath(int x, int y)
        {
            _map[x, y].SetType(MazeCellType.Floor);

            var directions = GetShuffledDirections();

            foreach (var dir in directions)
            {
                int nx = x + dir.x * 2;
                int ny = y + dir.y * 2;

                if (IsInBounds(nx, ny) && _map[nx, ny].Type == MazeCellType.Wall)
                {
                    // пробиваємо стіну між клітинками
                    _map[x + dir.x, y + dir.y].SetType(MazeCellType.Floor);
                    CarvePath(nx, ny);
                }
            }
        }

        private Vector2Int[] GetShuffledDirections()
        {
            var dirs = new[]
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            // Fisher–Yates shuffle
            for (int i = 0; i < dirs.Length; i++)
            {
                int j = _random.Next(i, dirs.Length);
                (dirs[i], dirs[j]) = (dirs[j], dirs[i]);
            }

            return dirs;
        }

        #endregion

        #region Exits

        /// <summary>
        /// Place exits. Logic will place exits just on right and top walls of the maze
        /// </summary>
        private void PlaceExits()
        {
            var candidates = new List<MazeCell>();
            int rightWallX = _width - 1;
            for (int y = 1; y < _height - 1; y++)
            {
                if (_map[rightWallX - 1, y].Type == MazeCellType.Floor)
                {
                    candidates.Add(_map[rightWallX, y]);
                }
            }

            int topWallY = _height - 1;
            for (int x = 1; x < _width - 1; x++)
            {
                if (_map[x, topWallY - 1].Type == MazeCellType.Floor)
                {
                    candidates.Add(_map[x, topWallY]);
                }
            }

            int exitsToPlace = Mathf.Min(_exitCount, candidates.Count);

            for (int i = 0; i < exitsToPlace; i++)
            {
                int index = _random.Next(candidates.Count);
                candidates[index].SetType(MazeCellType.Exit);
                candidates.RemoveAt(index);
            }
        }

        #endregion

        private bool IsInBounds(int x, int y)
        {
            return x > 0 && y > 0 && x < _width - 1 && y < _height - 1;
        }
    }

}