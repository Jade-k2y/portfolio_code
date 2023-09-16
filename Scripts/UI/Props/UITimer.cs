using UnityEngine;
using TMPro;


namespace Studio.Game
{
    public class UITimer : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _timer;
        [SerializeField]
        private float _duration;

        private float _elapsedTime;
        private bool _isRunning;


        private void Start() => ResetTimer();


        private void Update()
        {
            if (_isRunning)
            {
                _elapsedTime -= Time.deltaTime;

                if (_elapsedTime <= 0f)
                {
                    _elapsedTime = 0f;
                    _isRunning = false;
                }

                UpdateTimerText();
            }
        }


        public void StartTimer() => _isRunning = true;


        public void PauseTimer() => _isRunning = false;


        public void ResetTimer()
        {
            _elapsedTime = _duration;
            _isRunning = false;

            UpdateTimerText();
        }


        private void UpdateTimerText()
        {
            if (_timer)
            {
                var minutes = Mathf.FloorToInt(_elapsedTime / 60f);
                var seconds = Mathf.FloorToInt(_elapsedTime % 60f);
                var milliseconds = Mathf.FloorToInt((_elapsedTime * 100f) % 100f);

                _timer.SetText(string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds));
            }
        }
    }
}