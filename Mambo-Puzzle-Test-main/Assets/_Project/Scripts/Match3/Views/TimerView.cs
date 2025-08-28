using System;
using TMPro;
using UnityEngine;

namespace Match3.Views
{

    public class TimerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timerText;

        public void SetTime(float time)
        {
            string text = TimeSpan.FromSeconds(time).ToString(@"m\:ss");
            _timerText.text = text;
        }
    }
}
