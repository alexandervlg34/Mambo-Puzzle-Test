using Match3.Views;
using System;
using UI.Infractructure;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class ResultsWindow : BaseWindow
    {
        [SerializeField] private Button _confirmButton;
        [SerializeField] private ScoreView _scoreView;

        public event Action<ResultsWindow> ConfirmButtonClicked;

        private void Awake()
        {
            _confirmButton.onClick.AddListener(InvokeConfirmButtonClicked);
        }

        public void Initialize(int score)
        {
            _scoreView.SetScore(score);
        }

        private void InvokeConfirmButtonClicked()
        {
            ConfirmButtonClicked?.Invoke(this);
        }
    }
}
