using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Match3.Views
{
    public class Match3GridDebug : MonoBehaviour
    {
        [SerializeField] private int _gizmosFontSize = 20;
        private Match3View _match3View;
        private Cell[,] _cells;
        private CellView[,] _cellViews;
        private int _testCellsMatched = 0;
        private IEnumerator _testEnumerator;

        public void Init(Match3View match3View, Cell[,] cells, CellView[,] cellViews)
        {
            _match3View = match3View;
            _cells = cells;
            _cellViews = cellViews;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                bool testIsRunning = _testEnumerator != null;
                if (testIsRunning)
                {
                    _match3View.MatchAnimated -= IncreaseMatchedCells;
                    StopCoroutine(_testEnumerator);
                    _testEnumerator = null;
                }
                else
                {
                    StartCoroutine(_testEnumerator = AutoMatchesTest());
                }
            }
        }

        private IEnumerator AutoMatchesTest()
        {
            _match3View.MatchAnimated += IncreaseMatchedCells;
            System.Random random = new();

            while (_testCellsMatched < 10000)
            {
                _match3View.MakeRandomSwap(random);
                yield return null;
            }

            _match3View.MatchAnimated -= IncreaseMatchedCells;
            _testEnumerator = null;
        }

        private void IncreaseMatchedCells(int matchedCells)
        {
            _testCellsMatched += matchedCells;
        }

        private void OnDrawGizmosSelected()
        {
            if (_cells == null) return;

            int dataRows = _cells.GetLength(0);
            int dataColumns = _cells.GetLength(1);

            GUIStyle style = new();
            style.normal.textColor = Color.black;
            style.fontSize = _gizmosFontSize;

            for (int y = 0; y < dataColumns; y++)
            {
                for (int x = 0; x < dataRows; x++)
                {
                    CellView cellView = _cellViews[x, y];
                    Vector3 cellPosition = cellView.transform.position;
                    float xPosition = cellPosition.x;
                    float yPosition = cellPosition.y;

                    CellType type = _cells[x, y].Type;
                    string text = "";
                    switch (type)
                    {
                        case CellType.Type1: text += "B"; break;
                        case CellType.Type2: text += "G"; break;
                        case CellType.Type3: text += "O"; break;
                        case CellType.Type4: text += "R"; break;
                        case CellType.Empty: break;
                        default: throw new NotSupportedException($"\"{nameof(CellType)}\" value \"{type}\" is not supported");
                    }

                    Handles.Label(new Vector3(xPosition, yPosition, 0f), text, style);
                }
            }
        }
    }
}
