using GameCore.Entity.Player;

namespace GameCore.Entity.Factory
{
    public interface IPlayerFactory
    {
        PlayerEntityController Create();
    }
}