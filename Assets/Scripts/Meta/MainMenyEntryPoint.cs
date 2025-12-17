using Plugins.Core.UI;
using UI.Screens;
using UnityEngine;
using Zenject;

namespace Meta
{
    public class MainMenuEntryPoint : IInitializable
    {
        private IUIManager _uiManager;
        
        public MainMenuEntryPoint(IUIManager uiManager)
        {
            _uiManager = uiManager;
        }
        public void Initialize()
        {
            if(_uiManager.OpenedElementsCount == 0)
                _uiManager.Get<MainMenuScreen>().Show(null);
        }
    }
}