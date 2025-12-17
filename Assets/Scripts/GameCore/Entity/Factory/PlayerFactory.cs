using GameCore.Entity.Player;
using GameCore.Services.InputService;
using GameCore.Services.MazeControllerService;
using GameCore.Services.SceneControllerService;

namespace GameCore.Entity.Factory
{
    public class PlayerFactory : IPlayerFactory
    {
        private IInputService _inputService;
        private ISceneControllerService _sceneControllerService;
        private IMazeControllerService _mazeControllerService;

        public PlayerFactory(IInputService inputService, ISceneControllerService sceneControllerService, IMazeControllerService mazeControllerService)
        {
            _inputService = inputService;
            _sceneControllerService = sceneControllerService;
            _mazeControllerService = mazeControllerService;
        }

        public PlayerEntityController Create()
        {
            var playerView = _sceneControllerService.CreatePlayerView();
            var playerController = new PlayerEntityController(playerView, _inputService, _mazeControllerService);
            playerController.Initialize();
            return playerController;
        }
    }
}