using System;

namespace Plugins.Core.Common.Services.GameProgressSaveService
{
    [Serializable]
    public class GameResultData
    {
        public int Steps;
        public (int, int) TimeSpent;

        public GameResultData(int steps, (int, int) timeSpent)
        {
            Steps = steps;
            TimeSpent = timeSpent;
        }
    }
}