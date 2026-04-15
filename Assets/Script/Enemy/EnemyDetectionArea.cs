using UnityEngine;

namespace Script.Enemy
{
    public class EnemyDetectionArea : MonoBehaviour
    {
        private bool _inDetectionArea;
        private bool _startTiming;
        private float _timer;

        private void Update()
        {
            StartTimer();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;
            _startTiming = true;
            _timer = 1f;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) _inDetectionArea = true;
        }

        public bool GetDetectionArea() => _inDetectionArea;

        private void StartTimer()
        {
            if (!_startTiming) return;
            _timer -= Time.unscaledDeltaTime;
            if (!(_timer <= 0)) return;
            _inDetectionArea = false;
            _startTiming = false;
            _timer = 1f;
        }
    }
}