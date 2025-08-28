using System;
using UnityEngine;

namespace Match3.Views
{
    public interface IGameModeView : IDisposable
    {
        Match3View Match3View { get; }
        void Activate();
    }

    public class TimeModeView : MonoBehaviour, IGameModeView
    {
        [SerializeField] private Match3View _match3View;
        [SerializeField] private TimerView _timerView;
        [SerializeField] private ScoreView _scoreView;

        public Match3View Match3View => _match3View;
        public TimerView TimerView => _timerView;
        public ScoreView ScoreView => _scoreView;

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Dispose()
        {
            _match3View.Dispose();
            gameObject.SetActive(false);
        }
    }
}
