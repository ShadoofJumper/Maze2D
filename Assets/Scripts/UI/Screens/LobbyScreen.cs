using System;
using GameCore;
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

        [SerializeField] private TMP_InputField _heightInput;
        [SerializeField] private TMP_InputField _widthInput;
        [SerializeField] private TMP_InputField _exitNumberInput;

        [SerializeField] private TextMeshProUGUI _oversideWarningText;
        
        private IUIManager _uiManager;
        private GameSetupService _gameSetupService;
        private MazeConfig _currentMazeConfig;
        
        [Inject]
        public void Construct(IUIManager uiManager, GameSetupService gameSetupService)
        {
            _uiManager = uiManager;
            _gameSetupService = gameSetupService;
        }
        
        public override void Initialize()
        {
            SetupMazeSettingsView();
            SetupMazeWarningText();
            _currentMazeConfig = _gameSetupService.MazeConfig;
            _oversideWarningText.gameObject.SetActive(false);
        }
        
        private void SetupMazeSettingsView()
        {
            var config = _gameSetupService.MazeConfig;
            _heightInput.text = config.Height.ToString();
            _widthInput.text = config.Width.ToString();
            _exitNumberInput.text = config.NumberOfExits.ToString();
        }

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
            _playButton.onClick.RemoveListener(OnPlay);
        }
        
        private void OnPlay()
        {
            if (!TryGetInputMazeSettings())
            {
                _oversideWarningText.gameObject.SetActive(true);
                return;
            }
            _gameSetupService.SetCustomMazeConfig(_currentMazeConfig);
            _gameSetupService.Play();
            Hide();
        }
        
        private bool TryGetInputMazeSettings()
        {
            MazeConfig customMazeConfig = new MazeConfig()
            {
                Width = int.Parse(_widthInput.text),
                Height = int.Parse(_heightInput.text),
                NumberOfExits = int.Parse(_exitNumberInput.text)
            };

            if (!ValidateMazeConfig(customMazeConfig))
            {
                return false;
            }
            
            _currentMazeConfig = customMazeConfig;
            return true;
        }

        private bool ValidateMazeConfig(MazeConfig mazeConfig)
        {
            return mazeConfig.Height is <= GameConstants.MaxMazeHeight and >= GameConstants.MinMazeHeight 
                   && mazeConfig.Width is <= GameConstants.MaxMazeWidth and >= GameConstants.MinMazeWidth 
                   && mazeConfig.NumberOfExits is <= GameConstants.MaxMazeExitNumbers and >= GameConstants.MinMazeExitNumbers;
        }

        private void SetupMazeWarningText()
        {
            _oversideWarningText.text =
                $"Sorry, here are limits for the maze size: \n{GameConstants.MinMazeWidth - 1} > width < {GameConstants.MaxMazeWidth + 1}\n" +
                $" {GameConstants.MinMazeHeight - 1} > height < {GameConstants.MaxMazeHeight + 1}\n" +
                $" {GameConstants.MinMazeExitNumbers - 1} > number of exits < {GameConstants.MaxMazeExitNumbers + 1}.";
        }
        
        private void OnExitLobby()
        {
            _uiManager.Get<MainMenuScreen>().Show(null);
            Hide();
        }
    }
}
