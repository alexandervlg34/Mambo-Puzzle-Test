using Match3;
using Match3.Data;
using Match3.GameModes;
using Match3.Randomizers;
using Match3.Views;

namespace Infrastructure.GameDataProviers
{
    public class TimeModeGameDataProvider : IGameDataProvider
    {
        private readonly TimeModeView _timeModeView;
        private readonly ICellsRandomizer _cellsRandomizer;
        private readonly LevelsData _levelsData;
        private readonly int _levelCount;

        public TimeModeGameDataProvider(LevelsData levelsData, TimeModeView timeModeView, ICellsRandomizer cellsRandomizer) 
        {
            _levelsData = levelsData;
            _timeModeView = timeModeView;
            _cellsRandomizer = cellsRandomizer;
            _levelCount = levelsData.Data.Length;
        }

        public (Match3Board match3Board, IGameMode gameMode, IGameModeView gameModeView) GetData(int currentLevel)
        {
            Match3Board board = CreateBoard();
            IGameMode gameMode = CreateGameMode(currentLevel);
            IGameModeView gameModeView = _timeModeView;

            return (board, gameMode, gameModeView);
        }

        private Match3Board CreateBoard()
        {
            return new Match3Board(10, 10, _cellsRandomizer);
        }

        private IGameMode CreateGameMode(int currentLevel)
        {
            TimeBasedLevelData timeBasedLevelData = GetCurrentLevelConfig(currentLevel);
            return new TimeGameMode(timeBasedLevelData, _timeModeView);
        }

        private TimeBasedLevelData GetCurrentLevelConfig(int currentLevel)
        {
            int configIndex = currentLevel % _levelCount;
            return _levelsData.Data[configIndex];
        }
    }
}
