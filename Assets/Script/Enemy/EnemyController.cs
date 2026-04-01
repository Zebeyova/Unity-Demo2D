using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Script.Enemy
{
    public enum EnemyType
    {
        Guard,
        Patrol
    }

    public class EnemyController : MonoBehaviour
    {
        #region 属性

        public float baseSpeed = 0.5f;
        private Vector3 _startPosition;

        #endregion

        #region 成员

        public EnemyType enemyType;
        private Collider2D _cr2DEnemy;
        private Rigidbody2D _rb2dEnemy;
        private Transform _playerTransform;
        private SpriteRenderer _spriteRenderer;
        private EnemyDetectionArea _enemyDetectionArea;

        #endregion

        private void Awake()
        {
            CheckComponent();
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) _playerTransform = player.transform;
            _startPosition = transform.position;
        }

        private void FixedUpdate()
        {
            EnemyControl();
        }

        private void CheckComponent()
        {
            if (!_rb2dEnemy) _rb2dEnemy = GetComponent<Rigidbody2D>();
            if (!_cr2DEnemy) _cr2DEnemy = GetComponent<Collider2D>();
            if (!_spriteRenderer) _spriteRenderer = GetComponent<SpriteRenderer>();
            if (!_enemyDetectionArea) _enemyDetectionArea = GetComponentInChildren<EnemyDetectionArea>();
        }

        private void EnemyControl()
        {
            if (!_playerTransform) return;
            switch (enemyType)
            {
                case EnemyType.Guard:
                    GuardMove();
                    break;
                case EnemyType.Patrol:
                    PatrolMove();
                    break;
                default:
                    return;
            }
        }

        private void GuardMove()
        {
            var Distance = _enemyDetectionArea.GetDetectionArea()
                ? (_playerTransform.position - gameObject.transform.position).normalized
                : (_startPosition - gameObject.transform.position).normalized;

            _spriteRenderer.flipX = Vector3.Cross(Distance, gameObject.transform.forward).y > 0; //左边
            _rb2dEnemy.velocity = Distance * baseSpeed;
        }

        private void PatrolMove()
        {
        }
    }
}