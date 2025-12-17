using GameCore.Models;
using UnityEngine;
using Zenject;

namespace GameCore.Services.GameTimerService
{
    public class GameTimerService : IGameTimerService, IInitializable, ITickable
    {
        private GameSessionModel _gameSessionModel;
        
        private float _startTime;
        private float _elapsedTime;
        
        public int MinutesFromStart => Mathf.FloorToInt(_elapsedTime / 60);
        public int SecondsFromStart => Mathf.FloorToInt(_elapsedTime % 60);

        public GameTimerService(GameSessionModel gameSessionModel)
        {
            _gameSessionModel = gameSessionModel;
        }
        
        public void Initialize()
        {
            _startTime = Time.time;
        }
        
        public void Tick()
        {
            _elapsedTime = Time.time - _startTime;
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            _gameSessionModel.TimePast = (Mathf.FloorToInt(_elapsedTime / 60), Mathf.FloorToInt(_elapsedTime % 60));
        }
    }
}