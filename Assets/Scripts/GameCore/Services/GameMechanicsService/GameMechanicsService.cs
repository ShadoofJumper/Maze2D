using System;
using GameCore.Data;
using GameCore.Entity.Factory;
using GameCore.Entity.Player;
using GameCore.Models;
using GameCore.Services.GameTimerService;
using GameCore.Services.MazeControllerService;
using GameCore.Services.SceneControllerService;
using UnityEngine;

namespace GameCore.Services.GameMechanicsService
{
    /// <summary>
    /// Responsible for game mechanics logic and manage of the game progress
    /// </summary>
    public class GameMechanicsService : IGameMechanicsService, IDisposable
    {
        private MazeConfig _mazeConfig;
        private IMazeControllerService _mazeControllerService;
        private ISceneControllerService _sceneControllerService;
        private IPlayerFactory _playerFactory;
        private GameSessionModel _gameSessionModel;
        
        private PlayerEntityController _player;

        public Action OnGameComplete { get; set; }
        
        public GameMechanicsService(GameSetupService gameSetupService, 
            IMazeControllerService mazeControllerService, 
            ISceneControllerService sceneControllerService,
            IPlayerFactory playerFactory,
            GameSessionModel sessionModel)
        {
            _mazeControllerService = mazeControllerService;
            _mazeConfig = gameSetupService.MazeConfig;
            _sceneControllerService = sceneControllerService;
            _playerFactory = playerFactory;
            _gameSessionModel = sessionModel;
        }
        
        public void StartGame()
        {
            CreateMaze();
            CreatePlayer();
            SetCameraInMazeCenter();
        }
        
        private void CreatePlayer()
        {
            _player = _playerFactory.Create();
            var playerPos = _mazeControllerService.MazeCoordToWorld(_mazeControllerService.MazeCenter);
            _player.SetForcePosition(playerPos);
            _player.OnSuccessStep += OnSuccessStep;
        }

        private void OnSuccessStep(Vector2 mazeCell)
        {
            _gameSessionModel.Steps += 1;
            CheckForTheWin(mazeCell);
        }

        private void CheckForTheWin(Vector2 mazeCell)
        {
            if (_mazeControllerService.IsCellIsExit(mazeCell))
            {
                OnGameComplete?.Invoke();
            }
        }
        
        private void CreateMaze()
        {
            _mazeControllerService.CreateTheMaze(_mazeConfig);
        }

        private void SetCameraInMazeCenter()
        {
            _sceneControllerService.SetCameraPosition(_mazeControllerService.MazeCenter);
        }

        public void Dispose()
        {
            _player?.Dispose();
        }
    }
}