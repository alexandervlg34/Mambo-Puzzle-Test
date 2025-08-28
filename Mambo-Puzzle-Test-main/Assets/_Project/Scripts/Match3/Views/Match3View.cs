using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Match3.Views
{
    [Serializable]
    public class CellTypeViewPair
    {
        public CellType CellType;
        public CellContentView CellContent;
    }

    public class Match3View : MonoBehaviour, IDisposable
    {
        [SerializeField] private CellTypeViewPair[] _cellTypeViewPairs;
        [SerializeField] private CellView _cellViewPrefab;
        [SerializeField] private Transform _cellsParent;
        [SerializeField] private Transform _cellContentParent;
        [SerializeField] private Sprite _selectedCellSprite;
        [SerializeField] private Color _selectedCellSolor;

        private CellView[,] _cellViews;
        private Match3ViewInputHandler _inputHandler;

        private readonly List<Task> _collapseLineTasks = new(capacity: 20);
        private readonly YieldInstruction _destroyToCollapseDelayInstruction = new WaitForSeconds(FromDestroyToCollapseDelay);

        private const float CellSize = 0.8f;
        private const float FallUnitsPerSec = CellSize * 10;
        private const float FromDestroyToCollapseDelay = 0.3f;
        private const float CellShrinkDuration = 0.15f;

        public event CellsSwapDelegate CellsSwapTriggered;
        public event Action<int> MatchAnimated;

        public void Fill(Cell[,] cells)
        {
            int width = cells.GetLength(0);
            int height = cells.GetLength(1);

            CreateGrid(width, height, CellSize);
            CreateGridContent(cells);
            _inputHandler = new Match3ViewInputHandler(_cellViews);

            _inputHandler.CellsSwapTriggered += InvokeCellsSwap;
            _inputHandler.CellSelectionChanged += SetCellSelectedGraphics;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Match3GridDebug gridDebugComponent = gameObject.AddComponent<Match3GridDebug>();
            gridDebugComponent.Init(this, cells, _cellViews);
#endif
        }

        public void MakeSwap(MoveResult move, CellView swappableCell, CellView swapToCell, IEnumerable<CollapseResult> collapseResults)
        {
            if (move == MoveResult.OutOfBounds)
            {
                return;
            }

            CellContentView swappableCellContent = swappableCell.CellContentView;
            CellContentView swapToCellContent = swapToCell.CellContentView;

            Vector3 clickedCellPos = swappableCellContent.transform.position;
            Vector3 newlyClickedCellPos = swapToCellContent.transform.position;

            Action afterMove = null;

            if (move == MoveResult.NoMatch)
            {
                afterMove = MoveCellsBack;
            }

            if (move == MoveResult.Match)
            {
                swappableCell.CellContentView = swapToCellContent;
                swapToCell.CellContentView = swappableCellContent;
                afterMove = StartMakeMatches;
            }

            TurnOffInput();
            swappableCellContent.MoveTo(newlyClickedCellPos, FallUnitsPerSec, callback: afterMove);
            swapToCellContent.MoveTo(clickedCellPos, FallUnitsPerSec);

            void MoveCellsBack()
            {
                swappableCellContent.MoveTo(clickedCellPos, FallUnitsPerSec, callback: TurnOnInput);
                swapToCellContent.MoveTo(newlyClickedCellPos, FallUnitsPerSec);
            }

            void StartMakeMatches()
            {
                StartCoroutine(MakeMatches(collapseResults));
            }
        }

        public void MakeRandomSwap(System.Random random)
        {
            if (!_inputHandler.Enabled) return;

            int width = _cellViews.GetLength(0);
            int height = _cellViews.GetLength(1);
            int randomX = random.Next(0, width);
            int randomY = random.Next(0, height);
            CellView cellView = _cellViews[randomX, randomY];

            Direction direction = (Direction)random.Next(0, 4);
            if (!Match3Board.GetAdjacentCoords(height, width, randomX, randomY, direction, out int otherCellX, out int otherCellY))
            {
                return;
            }

            CellView otherCell = _cellViews[otherCellX, otherCellY];
            CellsSwapTriggered?.Invoke(cellView, otherCell);
        }

        public void Dispose()
        {
            _inputHandler.CellsSwapTriggered -= InvokeCellsSwap;
            _inputHandler.CellSelectionChanged -= SetCellSelectedGraphics;
            _inputHandler.Dispose();

            DestroyChildren(_cellsParent);
            DestroyChildren(_cellContentParent);
            _cellViews = null;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (TryGetComponent(out Match3GridDebug gridDataGizmo))
            {
                Destroy(gridDataGizmo);
            }
#endif
        }

        private void CreateGrid(int rows, int columns, float cellSize)
        {
            Vector3 position = _cellsParent.position;
            float startX = -(columns / 2.0f * cellSize) + (cellSize / 2);
            float startY = (rows / 2.0f * cellSize) - (cellSize / 2);
            startX += position.x;
            startY += position.y;

            _cellViews = new CellView[rows, columns];

            for (int y = 0; y < columns; y++)
            {
                for (int x = 0; x < rows; x++)
                {
                    float xPosition = startX + x * cellSize;
                    float yPosition = startY - y * cellSize;

                    CellView newCell = Instantiate(_cellViewPrefab, new Vector3(xPosition, yPosition, 0), Quaternion.identity, _cellsParent);
                    newCell.Initialize(x, y);
                    _cellViews[x, y] = newCell;
                }
            }
        }

        private void CreateGridContent(Cell[,] cells)
        {
            int width = _cellViews.GetLength(0);
            int height = _cellViews.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell cell = cells[x, y];
                    CellView cellView = _cellViews[x, y];

                    CellContentView contentView = CreateContentByType(cell.Type, cellView.transform.position);
                    _cellViews[x, y].CellContentView = contentView;
                }
            }
        }

        private void DestroyChildren(Transform parent)
        {
            int childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = parent.GetChild(0);
                DestroyImmediate(child.gameObject);
            }
        }

        private IEnumerator MakeMatches(IEnumerable<CollapseResult> collapseResults)
        {
            foreach (CollapseResult result in collapseResults)
            {
                Task matchAnimationTask = AnimateMatch(result.MatchedCells);
                yield return new WaitUntil(() => matchAnimationTask.IsCompleted);
                MatchAnimated?.Invoke(result.MatchedCells.Count);

                yield return _destroyToCollapseDelayInstruction;

                Task collapseTask = AnimateCollapses(result);
                yield return new WaitUntil(() => collapseTask.IsCompleted);
            }

            TurnOnInput();
        }

        private Task AnimateMatch(IEnumerable<(int x, int y)> matchedCellsCoords)
        {
            Task destroyCellTask = Task.CompletedTask;
            foreach ((int x, int y) in matchedCellsCoords)
            {
                CellView cellView = _cellViews[x, y];
                destroyCellTask = ShrinkAndDestroyCell(cellView); 
            }

            return destroyCellTask; // Since the duration is the same, we can only wait for one task
        }

        private async Task ShrinkAndDestroyCell(CellView cellView)
        {
            await cellView.CellContentView.AnimateShrink(CellShrinkDuration);
            Destroy(cellView.CellContentView.gameObject);
        }

        private Task AnimateCollapses(CollapseResult collapseResult)
        {
            _collapseLineTasks.Clear();
            foreach (CollapseLineData collapseLine in collapseResult.Collapes)
            {
                CreateCellsAndAnimateCollapse(collapseLine, _collapseLineTasks);
            }

            return Task.WhenAll(_collapseLineTasks);
        }

        private void CreateCellsAndAnimateCollapse(CollapseLineData collapseLine, List<Task> tasks)
        {
            int x = collapseLine.X;
            int newCellsCount = collapseLine.NewCells.Count;

            foreach ((int y, int fallToY) in collapseLine.CollapseCells) // existing cells fall
            {
                CellView cellView = _cellViews[x, y];
                CellView fallTo = _cellViews[x, fallToY];
                CellContentView fallContentView = cellView.CellContentView;
                AddFallAnimationTask(tasks, fallContentView, fallTo);
            }

            foreach ((int fallToY, CellType cellType) in collapseLine.NewCells) // creating new cells and animating fall
            {
                CellContentView cellContentView = CreateCellContentAboveGrid(x, newCellsCount, fallToY, cellType);
                CellView fallTo = _cellViews[x, fallToY];
                AddFallAnimationTask(tasks, cellContentView, fallTo);
            }

            static void AddFallAnimationTask(List<Task> tasks, CellContentView cellContentView, CellView fallTo)
            {
                Vector2 fallToPos = fallTo.transform.position;

                Task moveToTask = cellContentView.MoveTo(fallToPos, FallUnitsPerSec);
                tasks.Add(moveToTask);
                fallTo.CellContentView = cellContentView;
            }
        }

        private CellContentView CreateCellContentAboveGrid(int x, int newCellsCount, int fallToY, CellType cellType)
        {
            const int topYIndex = 0;
            CellView topCell = _cellViews[x, topYIndex];
            int cellsOffsetFromTop = newCellsCount - fallToY;

            Vector2 spawnPosition = topCell.transform.position;
            spawnPosition.y += CellSize * cellsOffsetFromTop;

            CellContentView cellContentView = CreateContentByType(cellType, spawnPosition);
            return cellContentView;
        }

        private CellContentView CreateContentByType(CellType cellType, Vector2 position)
        {
            CellContentView prefab = _cellTypeViewPairs.First(pair => pair.CellType == cellType).CellContent;
            return Instantiate(prefab, position, Quaternion.identity, _cellContentParent);
        }

        private void InvokeCellsSwap(CellView swappable, CellView swappableTo)
        {
            CellsSwapTriggered?.Invoke(swappable, swappableTo);
        }

        private void SetCellSelectedGraphics(CellView oldValue, CellView newValue)
        {
            if (oldValue != null)
            {
                oldValue.ResetVisual();
            }

            if (newValue != null)
            {
                newValue.SetSprite(_selectedCellSprite);
                newValue.SetSpriteColor(_selectedCellSolor);
            }
        }

        private void TurnOnInput() => _inputHandler.Enabled = true;
        private void TurnOffInput() => _inputHandler.Enabled = false;
    }
}
