using GameCore.Controllers.MazeBuilder;
using GameCore.Entity.Player;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameCore.Controllers
{
    public class SceneView: MonoBehaviour, ISceneView
    {
        //game is pretty simple, so we can store reference to prefabs here, but can move it in to scriptable object
        [SerializeField] private PlayerEntityView _playerEntityView;

        [Header("Tilemaps")]
        [SerializeField] private Tilemap _mazeTilemap;
        
        [Header("Tiles")]
        [SerializeField] private TileBase _freeTile;
        [SerializeField] private TileBase _wallTile;
        [SerializeField] private TileBase _exitTile;
        
        public PlayerEntityView CreatePlayerView()
        {
            return Instantiate(_playerEntityView);
        }

        public void CreateMazeCell(MazeCell mazeCell)
        {
            Vector3Int pos = new Vector3Int(mazeCell.X, mazeCell.Y, 0);

            TileBase tileToPaint;
            if (mazeCell.Type == MazeCellType.Exit)
            {
                tileToPaint = _exitTile;
            }else if (mazeCell.Type == MazeCellType.Wall)
            {
                tileToPaint = _wallTile;
            }else
            {
                tileToPaint = _freeTile;
            }
            _mazeTilemap.SetTile(pos, tileToPaint);
        }

        public void ClearMaze()
        {
            _mazeTilemap.ClearAllTiles();
        }
    }
}