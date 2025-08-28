using System;

namespace Match3.Views
{
    public delegate void CellsSwapDelegate(CellView swappable, CellView swappableTo);

    public class Match3ViewInputHandler : IDisposable
    {
        private CellView _currentlyHeldCell;
        private CellView _currentlyClickedCell;
        private readonly CellView[,] _cellViews;

        private CellView CurrentlyClickedCell
        {
            get => _currentlyClickedCell;
            set 
            {
                CellSelectionChanged?.Invoke(_currentlyClickedCell, value);
                _currentlyClickedCell = value;
            }
        }

        public bool Enabled { get; set; } = true;

        public event CellsSwapDelegate CellsSwapTriggered;
        public event Action<CellView, CellView> CellSelectionChanged;

        public Match3ViewInputHandler(CellView[,] cellViews)
        {
            _cellViews = cellViews;

            foreach (CellView cellView in cellViews)
            {
                cellView.MouseDown += CellView_MouseDown;
                cellView.MouseUp += CellView_MouseUp;
                cellView.MouseEnter += CellView_MouseEnter;
                cellView.MouseClicked += CellView_MouseClicked;
            }
        }

        public void Dispose()
        {
            foreach (CellView cellView in _cellViews)
            {
                cellView.MouseDown -= CellView_MouseDown;
                cellView.MouseUp -= CellView_MouseUp;
                cellView.MouseEnter -= CellView_MouseEnter;
                cellView.MouseClicked -= CellView_MouseClicked;
            }
        }

        private void CellView_MouseClicked(CellView cellView)
        {
            if (!Enabled) return;

            _currentlyHeldCell = null;

            if (CurrentlyClickedCell == null)
            {
                CurrentlyClickedCell = cellView;
                return;
            }

            if (CurrentlyClickedCell == cellView)
            {
                return;
            }

            if (!CurrentlyClickedCell.IsAdjacentTo(cellView))
            {
                CurrentlyClickedCell = cellView;
                return;
            }

            CellView swappableCell = _currentlyClickedCell;
            CurrentlyClickedCell = null;
            _currentlyHeldCell = null;

            CellsSwapTriggered?.Invoke(swappableCell, cellView);
        }

        private void CellView_MouseEnter(CellView cellView)
        {
            if (!Enabled) return;

            if (_currentlyHeldCell == null)
            {
                return;
            }

            if (_currentlyHeldCell == cellView)
            {
                return;
            }

            CellView swappableCell = _currentlyHeldCell;
            _currentlyHeldCell = null;
            CurrentlyClickedCell = null;
            CellsSwapTriggered?.Invoke(swappableCell, cellView);
        }

        private void CellView_MouseUp(CellView cellView)
        {
            if (!Enabled) return;
            _currentlyHeldCell = null;
        }

        private void CellView_MouseDown(CellView cellView)
        {
            if (!Enabled) return;

            if (_currentlyHeldCell == null)
            {
                _currentlyHeldCell = cellView;
            }
        }
    }
}
