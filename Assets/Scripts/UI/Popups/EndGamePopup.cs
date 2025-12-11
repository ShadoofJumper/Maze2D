using System;
using Plugins.Core.UI.Elements;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
    public sealed class EndGamePopupArgs
    {
        public readonly string SessionTime;
        public readonly string StepsUsed;

        public EndGamePopupArgs(string sessionTime, string stepsUsed)
        {
            SessionTime = sessionTime;
            StepsUsed = stepsUsed;
        }
    }
    public class EndGamePopup : PopupUIElement<EndGamePopupArgs, PopupOutputParameter>, IPopUp
    {
        [SerializeField] private TextMeshProUGUI _sessionTime;
        [SerializeField] private TextMeshProUGUI _stepsUsed;
        [SerializeField] private Button _restartButton;

        private const string _sessionTimeText = "Time: {0}";
        private const string _stepsUsedText = "Steps: {0}";

        public override void Initialize()
        {
            _restartButton.onClick.AsObservable().Subscribe(
                (x) => HideSuccessfully(PopupOutputParameter.Empty));
        }

        protected override void OnDataInitialized()
        {
            _sessionTime.text = String.Format(_sessionTimeText, Data.SessionTime);
            _stepsUsed.text =String.Format(_stepsUsedText, Data.StepsUsed);
        }

    }
}