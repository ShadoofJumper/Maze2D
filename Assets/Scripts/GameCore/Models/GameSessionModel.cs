using System;

namespace GameCore.Models
{
    public class GameSessionModel
    {
        public event Action<int> OnStepsChanged;
        public event Action<int, int> OnTimeChanged;

        private int _steps;
        private int _minutes;
        private int _seconds;

        public int Steps
        {
            get => _steps;
            set
            {
                if (_steps == value)
                    return;

                _steps = value;
                OnStepsChanged?.Invoke(_steps);
            }
        }

        public (int minutes, int seconds) TimePast
        {
            get => (_minutes, _seconds);
            set
            {
                if (_minutes == value.minutes && _seconds == value.seconds)
                    return;

                _minutes = value.minutes;
                _seconds = value.seconds;
                OnTimeChanged?.Invoke(_minutes, _seconds);
            }
        }
    }
}