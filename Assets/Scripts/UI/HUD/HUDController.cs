using System;
using GameCore;
using GameCore.Models;
using Plugins.Core.UI;
using UnityEngine.SceneManagement;
using Zenject;

namespace UI.HUD
{
    public class HUDController : IInitializable, IDisposable
    {
        private readonly GameSessionModel _model;
        private readonly HUDView _view;

        private IUIManager _uiManager;

        [Inject]
        public HUDController(GameSessionModel model, HUDView view)
        {
            _model = model;
            _view = view;
        }

        public void Initialize()
        {
            _view.OnExitClickAction += OnExitClicked;
            _model.OnStepsChanged += _view.SetSteps;
            _model.OnTimeChanged += _view.SetTime;
        }

        private void OnExitClicked()
        {
            SceneManager.LoadScene(GameConstants.MainMenuScene);
        }

        public void Dispose()
        {
            _model.OnStepsChanged -= _view.SetSteps;
            _model.OnTimeChanged -= _view.SetTime;
            
            _view.OnExitClickAction -= OnExitClicked;
        }
    }
}