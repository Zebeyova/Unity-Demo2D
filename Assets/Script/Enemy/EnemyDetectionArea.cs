using UnityEngine;

namespace Script.Enemy
{
    public class EnemyDetectionArea : MonoBehaviour
    {
        private bool _inDetectionArea;
        private float _timer;
        private bool _startTiming;
        public bool GetDetectionArea() => _inDetectionArea;

        private void Update()
        {
            StartTimer();
        }

        private void StartTimer()
        {
            if (!_startTiming) return;
            _timer -= Time.unscaledDeltaTime;
            if (_timer <= 0)
            {
                _inDetectionArea = false;
                _startTiming = false;
                _timer = 1f;
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) _inDetectionArea = true;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _startTiming = true;
                _timer = 1f;
            }
        }
    }
}