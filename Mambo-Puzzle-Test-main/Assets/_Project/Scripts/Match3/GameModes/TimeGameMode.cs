using Match3.Data;
using Match3.Views;

namespace Match3.GameModes
{
    public class TimeGameMode : IGameMode
    {
        private readonly TimerView _timerView;
        private readonly ScoreView _scoreView;
        private readonly TimeModeView _timerModeView;
        private float _currentTime;
        private int _score = 0;

        public event GameFinish Finished;

        public TimeGameMode(TimeBasedLevelData levelData, TimeModeView timeModeView)
        {
            _currentTime = levelData.TimeLimit;
            _timerView = timeModeView.TimerView;
            _scoreView = timeModeView.ScoreView;
            _timerModeView = timeModeView;
            _scoreView.SetScore(0);
            timeModeView.Match3View.MatchAnimated += IncreaseScore;
        }

        public void Update(float deltaTime)
        {
            _currentTime -= deltaTime;
            _timerView.SetTime(_currentTime);

            if (_currentTime <= 0f)
            {
                Finished?.Invoke(false, _score);
            }
        }

        public void Dispose()
        {
            _timerModeView.Dispose();
            _timerModeView.Match3View.MatchAnimated -= IncreaseScore;
            _timerView.SetTime(0f);
            _scoreView.SetScore(0);
        }

        private void IncreaseScore(int matchCount)
        {
            _score += matchCount;
            _scoreView.SetScore(_score);
        }
    }
}
