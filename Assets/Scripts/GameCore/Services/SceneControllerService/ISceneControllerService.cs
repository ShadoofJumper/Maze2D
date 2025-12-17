using GameCore.Controllers.MazeBuilder;
using GameCore.Entity.Player;
using UnityEngine;

namespace GameCore.Services.SceneControllerService
{
    /// <summary>
    /// Access point to scene view
    /// </summary>
    public interface ISceneControllerService
    {
        /// <summary>
        /// Create player monobehavior presentation on scene
        /// </summary>
        /// <returns></returns>
        public PlayerEntityView CreatePlayerView();

        /// <summary>
        /// Paint maze tile
        /// </summary>
        public void CreateMazeCell(MazeCell mazeCell);
        
        /// <summary>
        /// Clear maze tilemap
        /// </summary>
        public void ClearMazeMap();

        /// <summary>
        /// Set position of camera, can be used for position camera in center of the maze
        /// </summary>
        /// <param name="pos"></param>
        public void SetCameraPosition(Vector3 pos);
    }
}