using Plugins.Core.Common.Services.GameProgressSaveService;
using Plugins.Core.UI;
using Plugins.Core.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Screens
{
    public class MainMenuScreen : ScreenUIElement<object>
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _exitGame;
        [SerializeField] private TextMeshProUGUI _bestResultText;

        private IUIManager _uiManager;
        private IGameProgressService _playerPrefsGameProgressService;

        [Inject]
        public void Construct(IUIManager uiManager, IGameProgressService playerPrefsGameProgressService)
        {
            _uiManager = uiManager;
            _playerPrefsGameProgressService = playerPrefsGameProgressService;
        }
        
        public override void Initialize()
        {
        }
        
        protected override void OnShow()
        {
            base.OnShow();

            _playButton.onClick.AddListener(OnClickPlay);
            _exitGame.onClick.AddListener(OnExitGame);
            ShowBestResult();
        }

        protected override void OnHide()
        {
            base.OnHide();
            _playButton.onClick.RemoveListener(OnClickPlay);
            _exitGame.onClick.RemoveListener(OnExitGame);
        }

        private void ShowBestResult()
        {
            var bestResultData = _playerPrefsGameProgressService.Load();
            if (bestResultData == null)
            {
                return;
            }
            var steps = bestResultData.Steps;
            var time = $"{bestResultData.TimeSpent.Item1:00}:{bestResultData.TimeSpent.Item2:00}";
            _bestResultText.text = $"Steps: {steps}       Time: {time}";
        }

        private void OnClickPlay()
        {
            Hide();
            _uiManager.Get<LobbyScreen>().Show(null);
        }
        
        private void OnExitGame()
        {
            Application.Quit();
        }
    }

}
