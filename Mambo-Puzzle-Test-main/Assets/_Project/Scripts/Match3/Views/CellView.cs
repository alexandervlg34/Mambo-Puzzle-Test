using System;
using UnityEngine;

namespace Match3.Views
{
    public class CellView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private Color? _defaultColor;
        private Sprite _defaultSprite;

        public int X { get; private set; }
        public int Y { get; private set; }
        public CellContentView CellContentView { get; set; }

        public event Action<CellView> MouseDown;
        public event Action<CellView> MouseEnter;
        public event Action<CellView> MouseUp;
        public event Action<CellView> MouseClicked;

        public void Initialize(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void SetSprite(Sprite sprite)
        {
            if (_defaultSprite == null)
            {
                _defaultSprite = _spriteRenderer.sprite;
            }

            _spriteRenderer.sprite = sprite;
        }

        public void SetSpriteColor(Color color)
        {
            _defaultColor ??= _spriteRenderer.color;
            _spriteRenderer.color = color;
        }

        public void ResetVisual()
        {
            if (_defaultSprite != null)
            {
                _spriteRenderer.sprite = _defaultSprite;
            }

            if (_defaultColor != null)
            {
                _spriteRenderer.color = _defaultColor.Value;
            }
        }

        public bool IsAdjacentTo(CellView other)
        {
            int xDifference = Mathf.Abs(X - other.X);
            if (xDifference > 1)
            {
                return false;
            }

            int yDifference = Mathf.Abs(Y - other.Y);
            if (yDifference > 1)
            {
                return false;
            }

            if (xDifference == 1 && yDifference == 1)
            {
                return false;
            }

            return true;
        }

        private void OnMouseUpAsButton()
        {
            MouseClicked?.Invoke(this);
        }

        private void OnMouseDown()
        {
            MouseDown?.Invoke(this);
        }

        private void OnMouseEnter()
        {
            MouseEnter?.Invoke(this);
        }

        private void OnMouseUp()
        {
            MouseUp?.Invoke(this);
        }
    }
}
