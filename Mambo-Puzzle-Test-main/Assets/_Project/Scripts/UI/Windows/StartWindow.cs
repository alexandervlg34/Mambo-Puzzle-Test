using System;
using UI.Infractructure;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class StartWindow : BaseWindow
    {
        [SerializeField] private Button _startButton;

        public event Action<StartWindow> ButtonClicked;

        private void Awake()
        {
            _startButton.onClick.AddListener(InvokeStartButtonClicked);
        }

        private void InvokeStartButtonClicked()
        {
            ButtonClicked?.Invoke(this);
        }
    }
}
