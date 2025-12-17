using System;
using GameCore.Models;
using GameCore.Services.GameMechanicsService;
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

        public GameSessionService(IGameMechanicsService gameMechanicsService, 
            IUIManager uiManager,
            GameSessionModel gameSessionModel)
        {
            _gameMechanicsService = gameMechanicsService;
            _uiManager = uiManager;
            _gameSessionModel = gameSessionModel;
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
            _uiManager.Get<EndGamePopup>().Show(new EndGamePopupArgs(sessionTime, steps)).SubscribeOnClose(
                _ => RestartLevel());
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