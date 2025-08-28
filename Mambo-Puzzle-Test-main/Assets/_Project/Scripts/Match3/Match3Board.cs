using Match3.Randomizers;
using System;
using System.Collections.Generic;

namespace Match3
{
    public enum MoveResult
    {
        OutOfBounds,
        NoMatch,
        Match
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class CollapseLineData
    {
        public int X;
        public List<(int y, int fallToY)> CollapseCells = new();
        public List<(int fallToY, CellType cellType)> NewCells = new();
    }

    public class CollapseResult
    {
        public List<(int x, int y)> MatchedCells = new();
        public List<CollapseLineData> Collapes = new();
    }

    public class Match3Board
    {
        private readonly int _width;
        private readonly int _height;
        private readonly Cell[,] _cells;
        private readonly Random _random;
        private readonly ICellsRandomizer _cellsRandomizer;
        private readonly List<(int x, int y)> _tempMatchedCells = new();

        public Cell[,] Cells => _cells;
        public int Width => _width;
        public int Height => _height;

        public Match3Board(int width, int height, ICellsRandomizer cellsRandomizer)
        {
            _width = width;
            _height = height;
            _cells = new Cell[width, height];
            _random = new Random();
            _cellsRandomizer = cellsRandomizer;

            GenerateBoardWithoutMatches();
        }

        public MoveResult MoveCell(int x, int y, Direction direction, out List<CollapseResult> collapseResults)
        {
            collapseResults = null;

            if (!GetAdjacentCoords(_height, _width, x, y, direction, out int newX, out int newY))
            {
                return MoveResult.OutOfBounds;
            }

            return MoveCell(x, y, newX, newY, out collapseResults);
        }

        public static bool GetAdjacentCoords(int height, int width, int x, int y, Direction direction, out int newX, out int newY)
        {
            newX = x;
            newY = y;

            switch (direction)
            {
                case Direction.Up:
                    if (y >= height - 1)
                        return false;
                    newY = y + 1;
                    break;
                case Direction.Down:
                    if (y <= 0)
                        return false;
                    newY = y - 1;
                    break;
                case Direction.Left:
                    if (x <= 0)
                        return false;
                    newX = x - 1;
                    break;
                case Direction.Right:
                    if (x >= width - 1)
                        return false;
                    newX = x + 1;
                    break;
                default:
                    throw new NotSupportedException($"\"{nameof(Direction)}\" value \"{direction}\" is not supported");
            }

            return true;
        }

        public MoveResult MoveCell(int x, int y, int newX, int newY, out List<CollapseResult> collapseResults)
        {
            collapseResults = new();

            Cell targetCell = _cells[newX, newY];
            Cell movedCell = _cells[x, y];

            CellType movedCellType = movedCell.Type;
            CellType targetCellType = targetCell.Type;

            movedCell.Type = targetCellType;
            targetCell.Type = movedCellType;

            List<(int x, int y)> matchedCells = new();
            bool matchOccured;
            matchOccured = CheckMatch(newX, newY, matchedCells);
            matchOccured |= CheckMatch(x, y, matchedCells);

            while (matchOccured) // checking automatically occured matches
            {
                EmptyCells(matchedCells);
                CollapseResult collapseResult = CollapseAndRefill();
                collapseResult.MatchedCells = matchedCells;
                collapseResults.Add(collapseResult);

                matchedCells = CheckBoardForNewMatches();
                matchOccured = matchedCells.Count > 0;
            }

            if (collapseResults.Count > 0)
            {
                return MoveResult.Match;
            }

            movedCell.Type = movedCellType;
            targetCell.Type = targetCellType;

            return MoveResult.NoMatch;
        }

        public bool HasPossibleMatches()
        {
            int matchesCount = CheckBoardForNewMatches().Count;
            return matchesCount > 0;
        }

        public override string ToString()
        {
            // it's better to use stringbuilder
            string result = "";

            for (int y = _height - 1; y >= 0; y--)
            {
                string row = "";
                for (int x = 0; x < _width; x++)
                {
                    Cell cell = _cells[x, y];
                    row += cell.IsEmpty ? "." : ((int)cell.Type).ToString();
                    row += " ";
                }

                result += row + "\n";
            }

            return result;
        }

        private bool CheckMatch(int x, int y, List<(int x, int y)> matchedCells)
        {
            CellType targetType = _cells[x, y].Type;

            bool horizontalMatched = false;
            (int x, int y) checkingCellCoord = (x, y);

            _tempMatchedCells.Clear();
            _tempMatchedCells.Add(checkingCellCoord);
            horizontalMatched = TryMakeHorizontalMatch(x, y, horizontalMatched, _tempMatchedCells);

            _tempMatchedCells.Clear();
            _tempMatchedCells.Add(checkingCellCoord);
            TryMakeVerticalMatch(x, y, horizontalMatched, _tempMatchedCells);

            return matchedCells.Count != 0;

            bool TryMakeHorizontalMatch(int x, int y, bool horizontalMatched, List<(int x, int y)> match)
            {
                for (int i = x - 1; i >= 0 && _cells[i, y].Type == targetType; i--) // to the left
                {
                    match.Add((i, y));
                }
                for (int i = x + 1; i < _width && _cells[i, y].Type == targetType; i++) // to the right
                {
                    match.Add((i, y));
                }
                if (match.Count >= 3)
                {
                    matchedCells.AddRange(match);
                    horizontalMatched = true;
                }

                return horizontalMatched;
            }

            void TryMakeVerticalMatch(int x, int y, bool horizontalMatched, List<(int x, int y)> verticalMatch)
            {
                for (int j = y - 1; j >= 0 && _cells[x, j].Type == targetType; j--) // up
                {
                    verticalMatch.Add((x, j));
                }
                for (int j = y + 1; j < _height && _cells[x, j].Type == targetType; j++) // down
                {
                    verticalMatch.Add((x, j));
                }
                if (verticalMatch.Count >= 3)
                {
                    if (horizontalMatched)
                    {
                        verticalMatch.Remove((x, y));
                    }

                    matchedCells.AddRange(verticalMatch);
                }
            }
        }

        private void GenerateBoardWithoutMatches()
        {
            do
            {
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        do
                        {
                            CellType type = GetRandomCellType();
                            _cells[x, y] = new Cell(type);
                        }
                        while (HasInitialMatch(x, y));
                    }
                }
            } while (HasPossibleMatches());
        }

        private bool HasInitialMatch(int x, int y)
        {
            CellType targetType = _cells[x, y].Type;

            if (x >= 2 && _cells[x - 1, y].Type == targetType && _cells[x - 2, y].Type == targetType)
            {
                return true;
            }

            if (y >= 2 && _cells[x, y - 1].Type == targetType && _cells[x, y - 2].Type == targetType)
            {
                return true;
            }

            return false;
        }

        private void EmptyCells(IEnumerable<(int x, int y)> matchedCells)
        {
            foreach ((int x, int y) in matchedCells)
            {
                _cells[x, y].SetEmpty();
            }
        }

        private CollapseResult CollapseAndRefill()
        {
            CollapseResult collapseResult = new();
            for (int x = 0; x < _width; x++)
            {
                CollapseLine(x, out CollapseLineData collapseLineData, out int emptySlots);

                if (emptySlots == 0) continue;
                
                FillEmptyCells(x, collapseLineData, emptySlots);

                if (collapseLineData != null)
                {
                    collapseResult.Collapes.Add(collapseLineData);
                }
            }

            return collapseResult;

            void CollapseLine(int x, out CollapseLineData collapseLineData, out int emptySlots)
            {
                collapseLineData = null;
                emptySlots = 0;
                bool emptySlotsVisited = false;

                for (int y = _height - 1; y >= 0; y--)
                {
                    if (_cells[x, y].IsEmpty)
                    {
                        if (!emptySlotsVisited)
                        {
                            collapseLineData = new() { X = x };
                        }

                        emptySlots++;
                        emptySlotsVisited = true;
                    }
                    else if (emptySlotsVisited)
                    {
                        int fallToY = y + emptySlots;
                        _cells[x, fallToY].Type = _cells[x, y].Type;
                        _cells[x, y].SetEmpty();
                        collapseLineData.CollapseCells.Add((y, fallToY));
                    }
                }
            }

            void FillEmptyCells(int x, CollapseLineData collapseLineData, int emptySlots)
            {
                for (int y = 0; y < emptySlots; y++)
                {
                    CellType type = GetRandomCellType();
                    _cells[x, y].Type = type;
                    collapseLineData.NewCells.Add((y, type));
                }
            }
        }

        private List<(int x, int y)> CheckBoardForNewMatches()
        {
            List<(int x, int y)> newMatches = new();
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (_cells[x, y].IsEmpty == false && !newMatches.Contains((x, y)))
                    {
                        bool matchOccured = CheckMatch(x, y, newMatches);
                    }
                }
            }
            return newMatches;
        }

        private CellType GetRandomCellType()
        {
            return _cellsRandomizer.GetNext();
        }
    }
}
