using System;
using GameCore.Models;
using GameCore.Services.GameMechanicsService;
using Plugins.Core.Common.Services.GameProgressSaveService;
using Plugins.Core.UI;
using UI.Popups;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;


namespace GameCore.Services
{
    /// <summary>
    /// Service get config for game session
    /// Create session player
    /// manager game complete
    /// </summary>
    public class GameSessionService : IDisposable, IInitializable
    {
        private IGameMechanicsService _gameMechanicsService;
        private IUIManager _uiManager;
        private GameSessionModel _gameSessionModel;
        private IGameProgressService _gameProgressService;

        public GameSessionService(IGameMechanicsService gameMechanicsService, 
            IUIManager uiManager,
            GameSessionModel gameSessionModel,
            IGameProgressService gameProgressService)
        {
            _gameMechanicsService = gameMechanicsService;
            _uiManager = uiManager;
            _gameSessionModel = gameSessionModel;
            _gameProgressService = gameProgressService;
        }

        public void Initialize()
        {
            _gameMechanicsService.StartGame();
            _gameMechanicsService.OnGameComplete += GameComplete;
        }

        private void GameComplete()
        {
            var sessionTime = $"{_gameSessionModel.TimePast.minutes:00}:{_gameSessionModel.TimePast.seconds:00}";
            var steps = _gameSessionModel.Steps.ToString();
            SaveIfBestResult();
            _uiManager.Get<EndGamePopup>().Show(new EndGamePopupArgs(sessionTime, steps)).SubscribeOnClose(
                _ => RestartLevel());
        }

        private void SaveIfBestResult()
        {
            if (IsBestResult(_gameSessionModel.Steps, _gameSessionModel.TimePast))
            {
                _gameProgressService.Save(new GameResultData(_gameSessionModel.Steps, _gameSessionModel.TimePast));
            }
        }

        private bool IsBestResult(int steps, (int, int) sessionTime)
        {
            var oldResult = _gameProgressService.Load();
            if (oldResult == null)
            {
                return true;
            }
            var oldTimeInSeconds = oldResult.TimeSpent.Item1 * 60 + oldResult.TimeSpent.Item2;
            var newTimeInSeconds = sessionTime.Item1 * 60 + sessionTime.Item2;

            //check if all params is better, for simplicity
            return steps < oldResult.Steps  && newTimeInSeconds < oldTimeInSeconds;
        }
        
        private void RestartLevel()
        {
            SceneManager.LoadScene(Application.loadedLevel);
        }

        public void Dispose()
        {
            
        }
    }
}