using System;
using GameCore.Controllers;
using GameCore.Controllers.MazeBuilder;
using GameCore.Entity.Player;
using UnityEngine;

namespace GameCore.Services.SceneControllerService
{
    public class SceneControllerService : ISceneControllerService
    {
        private ISceneView _sceneView;
        private PlayerEntityView _playerEntityView;

        public SceneControllerService(ISceneView sceneView)
        {
            _sceneView = sceneView;
        }

        public PlayerEntityView CreatePlayerView()
        {
            var playerEntityView = _sceneView.CreatePlayerView();
            return playerEntityView;
        }

        public void SetCameraPosition(Vector3 pos)
        {
            //for simplicity we reference main camera
            var camera = Camera.main;
            if (camera == null)
            {
                throw new Exception("Camera does not exist at this moment!");
            }
            camera.transform.position = new Vector3(pos.x, pos.y, camera.transform.position.z);
        }

        public void CreateMazeCell(MazeCell mazeCell)
        {
            _sceneView.CreateMazeCell(mazeCell);
        }
        
        public void ClearMazeMap()
        {
            _sceneView.ClearMaze();
        }
    }
}