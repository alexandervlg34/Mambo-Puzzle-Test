using TMPro;
using UnityEngine;

namespace Match3.Views
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreText;
        private string _startingText;

        public void SetScore(int score)
        {
            _startingText ??= _scoreText.text;
            _scoreText.text = string.Format(_startingText, score);
        }
    }
}
