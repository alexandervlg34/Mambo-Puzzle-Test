using Match3.GameModes;
using Match3.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class Match3Controller : MonoBehaviour
    {
        private Match3Board _board;
        private Match3View _view;
        private IGameMode _gameMode;
        private IEnumerator _gameUpdateRoutine;

        public event GameFinish GameFinished;
    
        public void StartGame(Match3Board board, Match3View match3View, IGameMode gameMode)
        {
            _board = board;
            _view = match3View;
            _gameMode = gameMode;

            match3View.Fill(board.Cells);

            match3View.CellsSwapTriggered += TrySwapCells;
            _gameMode.Finished += InvokeGameFinished;

            _gameUpdateRoutine = UpdateGameModeRoutine();
            StartCoroutine(_gameUpdateRoutine);
        }

        private void InvokeGameFinished(bool status, int score)
        {
            StopCoroutine(_gameUpdateRoutine);
            _gameUpdateRoutine = null;

            _view.CellsSwapTriggered -= TrySwapCells;
            _gameMode.Finished -= InvokeGameFinished;

            GameFinished?.Invoke(status, score);
        }

        private void TrySwapCells(CellView swappable, CellView swappableTo)
        {
            if (!swappable.IsAdjacentTo(swappableTo))
            {
                return;
            }

            MoveResult moveResult = _board.MoveCell(swappable.X, swappable.Y, swappableTo.X, swappableTo.Y, out List<CollapseResult> collapseResults);
            _view.MakeSwap(moveResult, swappable, swappableTo, collapseResults);
        }

        private IEnumerator UpdateGameModeRoutine()
        {
            while (true)
            {
                _gameMode.Update(Time.deltaTime);
                yield return null;
            }
        }
    }
}
