using UnityEngine;

namespace Script.Enemy
{
    public class EnemyDetectionArea : MonoBehaviour
    {
        private bool _inDetectionArea;
        public bool GetDetectionArea() => _inDetectionArea;

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) _inDetectionArea = true;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) _inDetectionArea = false;
        }
    }
}