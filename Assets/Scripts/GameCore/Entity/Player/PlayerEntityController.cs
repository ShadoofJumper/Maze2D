using System;
using GameCore.Services.InputService;
using GameCore.Services.MazeControllerService;
using UnityEngine;
using Zenject;

namespace GameCore.Entity.Player
{
    public class PlayerEntityController: IInitializable, IDisposable
    {
        private IInputService _inputService;
        private IMazeControllerService _mazeControllerService;

        private PlayerEntityView _playerEntityView;

        public Action<Vector2> OnSuccessStep;
        
        public PlayerEntityController(PlayerEntityView playerEntityView, IInputService inputService, IMazeControllerService mazeControllerService)
        {
            _inputService = inputService;
            _playerEntityView = playerEntityView;
            _mazeControllerService = mazeControllerService;
        }
        
        public void Initialize()
        {
            _inputService.OnMoveDirection += TryMoveInDirection;
        }

        public void SetForcePosition(Vector3 position)
        {
            _playerEntityView.transform.position = position;
        }

        private void TryMoveInDirection(Vector2 dir)
        {
            if (_playerEntityView.IsMoving)
            {
                return;
            }
            var targetPos = _playerEntityView.transform.position + new Vector3(dir.x,dir.y,0);
            var targetCell = _mazeControllerService.WorldPosToMaze(targetPos);
            if (_mazeControllerService.IsCellFree(targetCell))
            {
                var playerNewPos = _mazeControllerService.MazeCoordToWorld(targetCell);
                _playerEntityView.MoveTo(playerNewPos);
                OnSuccessStep?.Invoke(targetCell);
            }
        }

        public void Dispose()
        {
            _inputService.OnMoveDirection -= TryMoveInDirection;
        }
    }
}