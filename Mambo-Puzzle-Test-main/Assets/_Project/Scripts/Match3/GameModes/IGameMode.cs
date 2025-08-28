using System;

namespace Match3.GameModes
{
    public delegate void GameFinish(bool won, int score);

    public interface IGameMode : IDisposable
    {
        event GameFinish Finished;

        void Update(float deltaTime);
    }
}
