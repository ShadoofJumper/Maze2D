using System;
using GameCore.Services.MazeControllerService;
using UnityEngine;
using Zenject;

namespace GameCore.Services.InputService
{
    public class InputService: IInputService, ITickable
    {
        public event Action<Vector2> OnMoveDirection;
        public event Action<Vector2> OnMovePosition;

        private IMazeControllerService _mazeControllerService;

        public InputService(IMazeControllerService mazeControllerService)
        {
            _mazeControllerService = mazeControllerService;
        }

        public void Tick()
        {
            HandleKeyboard();
        }

        private void HandleKeyboard()
        {
            float x = 0f;
            float y = 0f;

            var mazeCellSize = _mazeControllerService.MazeCellSize;
            if (Input.GetKey(KeyCode.A)) x -= mazeCellSize;
            if (Input.GetKey(KeyCode.D)) x += mazeCellSize;
            if (Input.GetKey(KeyCode.S)) y -= mazeCellSize;
            if (Input.GetKey(KeyCode.W)) y += mazeCellSize;

            Vector2 direction = new Vector2(x, y);

            if (direction.sqrMagnitude > 0f)
                OnMoveDirection?.Invoke(direction.normalized);
        }
    }
}