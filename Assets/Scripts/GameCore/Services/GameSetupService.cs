using GameCore.Data;
using UnityEngine.SceneManagement;
using Zenject;

namespace GameCore.Services
{
    public class GameSetupService : IInitializable
    {
        private MazeConfig _mazeConfig;
        public MazeConfig MazeConfig => _mazeConfig;

        public void Initialize()
        {
            //set default maze config, can be loaded from saved memory here
            _mazeConfig= new MazeConfig()
            {
                Height = GameConstants.DefaultHeight,
                Width = GameConstants.DefaultWidth,
                NumberOfExits = GameConstants.DefaultExit
            };
        }

        public void SetCustomMazeConfig(MazeConfig mazeConfig)
        {
            //here we can also save in to the memory custom maze config
            _mazeConfig = mazeConfig;
        }

        public void Play()
        {
            SceneManager.LoadScene(GameConstants.GamePlayScene, LoadSceneMode.Single);
        }
    }
}