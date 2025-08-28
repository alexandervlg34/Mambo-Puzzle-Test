using Match3;
using Match3.GameModes;
using Match3.Views;

namespace Infrastructure.GameDataProviers
{
    public interface IGameDataProvider
    {
        public (Match3Board match3Board, IGameMode gameMode, IGameModeView gameModeView) GetData(int currentLevel);
    }
}
