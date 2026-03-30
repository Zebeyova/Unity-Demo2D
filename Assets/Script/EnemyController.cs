using UnityEngine;

namespace Script
{
    public enum EnemyType
    {
        Guard,
        Patrol
    }

    public class EnemyController : MonoBehaviour
    {
        private Rigidbody2D _rb2dEnemy;
        private Collider2D _cr2DEnemy;
        public EnemyType enemyType = EnemyType.Guard;

        private void Awake()
        {
            CheckComponent();
        }

        private void CheckComponent()
        {
            if (!_rb2dEnemy) _rb2dEnemy = GetComponent<Rigidbody2D>();
            if (!_cr2DEnemy) _cr2DEnemy = GetComponent<Collider2D>();
        }

        private void FixedUpdate()
        {
            EnemyControl();
        }

        private void EnemyControl()
        {
            switch (enemyType)
            {
                case EnemyType.Guard:
                    break;
                case EnemyType.Patrol:
                    break;
            }
        }
    }
}