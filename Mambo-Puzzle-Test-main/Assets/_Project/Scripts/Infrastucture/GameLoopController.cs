using Infrastructure.GameDataProviers;
using Match3;
using Match3.GameModes;
using Match3.Views;
using UI.Infractructure;
using UI.Windows;

namespace Infrastructure
{
    public class GameLoopController
    {
        private readonly Match3Controller _match3Controller;
        private readonly UIManager _uiManager;
        private readonly IGameDataProvider _gameDataProvider;
        private IGameMode _currentGameMode;
        private int _currentLevel = 0;

        public GameLoopController(Match3Controller match3Controller, UIManager uiManager, IGameDataProvider gameDataProvider) 
        {
            _match3Controller = match3Controller;
            _uiManager = uiManager;
            _gameDataProvider = gameDataProvider;
        }

        public void Start()
        {
            _match3Controller.GameFinished += EnterResultsState;
            EnterStartState();
        }

        private void EnterStartState()
        {
            StartWindow startWindow = _uiManager.ShowWindow<StartWindow>();
            startWindow.ButtonClicked += HideStartAndEnterGame;
        }

        private void EnterGameState()
        {
            (Match3Board match3Board, IGameMode gameMode, IGameModeView gameModeView) = _gameDataProvider.GetData(_currentLevel);
            _currentGameMode = gameMode; 
            _match3Controller.StartGame(match3Board, gameModeView.Match3View, gameMode);
            gameModeView.Activate();
        }

        private void EnterResultsState(bool won, int score)
        {
            _currentGameMode.Dispose();
            _currentLevel++;

            ResultsWindow resultsWindow = _uiManager.ShowWindow<ResultsWindow>();
            resultsWindow.Initialize(score);
            resultsWindow.ConfirmButtonClicked += HideResultsAndEnterStart;
        }

        private void HideStartAndEnterGame(StartWindow startWindow)
        {
            startWindow.ButtonClicked -= HideStartAndEnterGame;
            _uiManager.HideWindow(startWindow);
            EnterGameState();
        }

        private void HideResultsAndEnterStart(ResultsWindow resultsWindow)
        {
            resultsWindow.ConfirmButtonClicked -= HideResultsAndEnterStart;
            _uiManager.HideWindow(resultsWindow);
            EnterStartState();
        }
    }
}
