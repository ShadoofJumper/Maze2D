using Plugins.Core.UI;
using Plugins.Core.UI.Elements;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Screens
{
    public class MainMenuScreen : ScreenUIElement<object>
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingButton;
        [SerializeField] private Button _exitGame;

        private IUIManager _uiManager;

        [Inject]
        public void Construct(IUIManager uiManager)
        {
            _uiManager = uiManager;
        }
        
        public override void Initialize()
        {
        }
        
        protected override void OnShow()
        {
            base.OnShow();

            _playButton.onClick.AddListener(OnClickPlay);
            _settingButton.onClick.AddListener(OnClickSettings);
            _exitGame.onClick.AddListener(OnExitGame);
        }

        protected override void OnHide()
        {
            base.OnHide();
            _playButton.onClick.RemoveListener(OnClickPlay);
            _settingButton.onClick.RemoveListener(OnClickSettings);
            _exitGame.onClick.RemoveListener(OnExitGame);
        }

        private void OnClickPlay()
        {
            _uiManager.Get<LobbyScreen>().Show(null);
            Hide();
        }
        private void OnClickSettings()
        {
            //call ui manager to show settings popup
        }

        private void OnExitGame()
        {
            Application.Quit();
        }
    }

}
