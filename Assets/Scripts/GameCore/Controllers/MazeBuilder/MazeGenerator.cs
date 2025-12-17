using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Controllers.MazeBuilder
{
    public class MazeGenerator
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _exitCount;

        private MazeCell[,] _map;
        private readonly System.Random _random = new System.Random();

        public Vector2Int StartCell { get; private set; }

        public MazeGenerator(int width, int height, int exitCount)
        {
            _width = MakeOdd(width);
            _height = MakeOdd(height);
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
            StartCell = CalculateCenterStart();

            if (!IsInBounds(StartCell.x, StartCell.y))
                throw new Exception("Maze too small for center start position");

            CarvePath(StartCell.x, StartCell.y);
        }

        private void CarvePath(int x, int y)
        {
            _map[x, y].SetType(MazeCellType.Floor);

            foreach (var dir in GetShuffledDirections())
            {
                int nx = x + dir.x * 2;
                int ny = y + dir.y * 2;

                if (IsInBounds(nx, ny) && _map[nx, ny].Type == MazeCellType.Wall)
                {
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

            for (int i = 0; i < dirs.Length; i++)
            {
                int j = _random.Next(i, dirs.Length);
                (dirs[i], dirs[j]) = (dirs[j], dirs[i]);
            }

            return dirs;
        }

        #endregion

        #region Exits

        private void PlaceExits()
        {
            var candidates = new List<MazeCell>();

            int rightWallX = _width - 1;
            for (int y = 1; y < _height - 1; y++)
            {
                if (_map[rightWallX - 1, y].Type == MazeCellType.Floor)
                    candidates.Add(_map[rightWallX, y]);
            }

            int leftWallX = 0;
            for (int y = 1; y < _height - 1; y++)
            {
                if (_map[leftWallX + 1, y].Type == MazeCellType.Floor)
                    candidates.Add(_map[leftWallX, y]);
            }

            int topWallY = _height - 1;
            for (int x = 1; x < _width - 1; x++)
            {
                if (_map[x, topWallY - 1].Type == MazeCellType.Floor)
                    candidates.Add(_map[x, topWallY]);
            }

            int bottomWallY = 0;
            for (int x = 1; x < _width - 1; x++)
            {
                if (_map[x, bottomWallY + 1].Type == MazeCellType.Floor)
                    candidates.Add(_map[x, bottomWallY]);
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

        #region Helpers

        private Vector2Int CalculateCenterStart()
        {
            int cx = _width / 2;
            int cy = _height / 2;

            // Snap to odd coordinates
            if (cx % 2 == 0) cx--;
            if (cy % 2 == 0) cy--;

            return new Vector2Int(cx, cy);
        }

        private static int MakeOdd(int value)
        {
            return value % 2 == 0 ? value + 1 : value;
        }

        private bool IsInBounds(int x, int y)
        {
            return x > 0 && y > 0 && x < _width - 1 && y < _height - 1;
        }

        #endregion
    }
}
