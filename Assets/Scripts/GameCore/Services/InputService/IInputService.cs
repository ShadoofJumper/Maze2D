using System;
using UnityEngine;

namespace GameCore.Services.InputService
{
    public interface IInputService
    {
        public event Action<Vector2> OnMoveDirection;
    }
}