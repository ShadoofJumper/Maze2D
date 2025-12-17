using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD
{
    public class HUDView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timer;
        [SerializeField] private TextMeshProUGUI _stepsCounter;

        [SerializeField] private Button _buttonExit;

        public event Action OnExitClickAction;

        private void Awake()
        {
            _buttonExit.onClick.AddListener(OnExitClick);
        }

        public void SetTime(int minutes, int seconds)
        {
            _timer.text = $"{minutes:00}:{seconds:00}";
        }
        
        public void SetSteps(int stepsCount)
        {
            _stepsCounter.text = $"Steps: {stepsCount.ToString()}";
        }

        private void OnExitClick()
        {
            OnExitClickAction?.Invoke();
        }

        private void OnDestroy()
        {
            _buttonExit.onClick.RemoveListener(OnExitClick);
        }
    }
}