using GameCore.Data;
using UnityEngine;

namespace GameCore.Services.MazeControllerService
{
    public interface IMazeControllerService
    {
        public float MazeCellSize { get; }
        public Vector3 MazeCenter { get; }
        public void CreateTheMaze(MazeConfig mazeConfig);
        public bool IsCellFree(Vector2 cellPos);
        public bool IsCellIsExit(Vector2 cellPos);
        public Vector3 MazeCoordToWorld(Vector2 cellPos);
        public Vector2 WorldPosToMaze(Vector3 cellPos);
    }
}