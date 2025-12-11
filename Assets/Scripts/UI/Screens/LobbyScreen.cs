using GameCore.Data;
using GameCore.Services;
using Plugins.Core.UI;
using Plugins.Core.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Screens
{
    public class LobbyScreen: ScreenUIElement<object>
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _playButton;

        [SerializeField] private TextMeshProUGUI _heightText;
        [SerializeField] private Button _changeHeightUp;
        [SerializeField] private Button _changeHeightDown;

        [SerializeField] private TextMeshProUGUI _widthText;
        [SerializeField] private Button _changeWidthUp;
        [SerializeField] private Button _changeWidthDown;

        [SerializeField] private TextMeshProUGUI _numberOfExitsText;
        [SerializeField] private Button _changeExitsUp;
        [SerializeField] private Button _changeExitsDown;

        
        private IUIManager _uiManager;
        private GameSetupService _gameSetupService;
        private MazeConfig _customMazeConfig;

        [Inject]
        public void Construct(IUIManager uiManager, GameSetupService gameSetupService)
        {
            _uiManager = uiManager;
            _gameSetupService = gameSetupService;
        }
        
        public override void Initialize()
        {
            SetupMazeSettingsView();
            _customMazeConfig = _gameSetupService.MazeConfig;
        }

        private void SetupMazeSettingsView()
        {
            var config = _gameSetupService.MazeConfig;
            _heightText.text = config.Height.ToString();
            _widthText.text = config.Width.ToString();
            _numberOfExitsText.text = config.NumberOfExits.ToString();
        }
        
        
        //TODO: here logic for maze config change

        protected override void OnShow()
        {
            base.OnShow();

            _exitButton.onClick.AddListener(OnExitLobby);
            _playButton.onClick.AddListener(OnPlay);
        }

        protected override void OnHide()
        {
            base.OnHide();
            _exitButton.onClick.RemoveListener(OnExitLobby);
            _playButton.onClick.AddListener(OnPlay);
        }
        
        private void OnPlay()
        {
            _gameSetupService.SetCustomMazeConfig(_customMazeConfig);
            _gameSetupService.Play();
            Hide();
        }

        private void OnExitLobby()
        {
            _uiManager.Get<MainMenuScreen>().Show(null);
            Hide();
        }

    }
}