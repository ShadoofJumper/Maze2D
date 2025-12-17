using GameCore.Controllers.MazeBuilder;
using GameCore.Entity.Player;

namespace GameCore.Controllers
{
    public interface ISceneView
    {
        public PlayerEntityView CreatePlayerView();

        public void CreateMazeCell(MazeCell mazeCell);
        
        public void ClearMaze();
    }
}