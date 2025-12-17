namespace GameCore.Services.GameTimerService
{
    public interface IGameTimerService
    {
        int MinutesFromStart { get; }
        int SecondsFromStart { get; }
    }
}