using GameCore.Data;
using UnityEngine.SceneManagement;
using Zenject;

namespace GameCore.Services
{
    public class GameSetupService : IInitializable
    {
        private const string GamePlayScene = "GamePlayScene";
        
        private MazeConfig _mazeConfig;
        public MazeConfig MazeConfig => _mazeConfig;

        public void Initialize()
        {
            //set default maze config, can be loaded from saved memory here
            _mazeConfig= new MazeConfig()
            {
                Height = 5,
                Width = 5,
                NumberOfExits = 2
            };
        }

        public void SetCustomMazeConfig(MazeConfig mazeConfig)
        {
            //here we can also save in to the memory custom maze config
            _mazeConfig = mazeConfig;
        }

        public void Play()
        {
            SceneManager.LoadScene(GamePlayScene);
        }
    }
}