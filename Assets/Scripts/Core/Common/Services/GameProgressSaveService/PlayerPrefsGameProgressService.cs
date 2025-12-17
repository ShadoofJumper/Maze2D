using Plugins.Core.Common.Services.GameProgressSaveService;
using UnityEngine;

public class PlayerPrefsGameProgressService : IGameProgressService
{
    private const string StepsKey = "GameResult_Steps";
    private const string TimeKeyM = "GameResult_TimeMinutes";
    private const string TimeKeyS = "GameResult_TimeSeconds";

    public void Save(GameResultData result)
    {
        PlayerPrefs.SetInt(StepsKey, result.Steps);
        PlayerPrefs.SetInt(TimeKeyM, result.TimeSpent.Item1);
        PlayerPrefs.SetInt(TimeKeyS, result.TimeSpent.Item2);
        PlayerPrefs.Save();
    }

    public GameResultData Load()
    {
        if (!HasSave())
            return null;

        int steps = PlayerPrefs.GetInt(StepsKey);
        (int,int) time = (PlayerPrefs.GetInt(TimeKeyM), PlayerPrefs.GetInt(TimeKeyS));

        return new GameResultData(steps, time);
    }

    public bool HasSave()
    {
        return PlayerPrefs.HasKey(StepsKey) && PlayerPrefs.HasKey(TimeKeyM) && PlayerPrefs.HasKey(TimeKeyS);
    }

    public void Clear()
    {
        PlayerPrefs.DeleteKey(StepsKey);
        PlayerPrefs.DeleteKey(TimeKeyM);
        PlayerPrefs.DeleteKey(TimeKeyS);
        PlayerPrefs.Save();
    }
}