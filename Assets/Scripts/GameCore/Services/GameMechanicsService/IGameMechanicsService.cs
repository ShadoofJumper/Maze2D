using System;

namespace GameCore.Services.GameMechanicsService
{
    public interface IGameMechanicsService
    {
        public Action OnGameComplete { get; set; }
        public void StartGame();
    }
}