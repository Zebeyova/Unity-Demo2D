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
        private void Awake()
        {
            CheckComponent();
            var player = GameObject.FindWithTag("Player");
            if (player != null) _playerTransform = player.transform;
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
            if (!_eAnimationController) _eAnimationController = GetComponent<EnemyAnimationController>();
            if (!_eDetectionArea) _eDetectionArea = GetComponentInChildren<EnemyDetectionArea>();
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
            Vector3 Distance;
            if (_startPosition == Vector3.zero) _startPosition = transform.position;
            if (_eDetectionArea.GetDetectionArea()) //追击状态
            {
                Distance = _playerTransform.position - gameObject.transform.position;
                transform.Rotate(Vector3.up, Vector3.Cross(Distance, gameObject.transform.forward).y > 0 ? 180 : 0,
                    Space.Self); //左边
                if (Distance.magnitude < 1.2f)
                {
                    _rb2dEnemy.velocity = Vector3.zero;
                    _eAnimationController.AttackAnimation();
                    return;
                }

                Distance = Distance.normalized;
                _eAnimationController.WalkAnimation();
                _rb2dEnemy.velocity = Distance * baseSpeed;
            }
            else //返回守卫状态
            {
                Distance = (_startPosition - gameObject.transform.position).normalized;
                transform.Rotate(Vector3.up, Vector3.Cross(Distance, gameObject.transform.forward).y > 0 ? 180 : 0,
                    Space.Self); //左边
                if (Vector3.Distance(_startPosition, gameObject.transform.position) < endError)
                {
                    _rb2dEnemy.velocity = Vector3.zero;
                    _eAnimationController.IdleAnimation();
                    return;
                }

                _eAnimationController.WalkAnimation();
                _rb2dEnemy.velocity = Distance * baseSpeed;
            }
        }

        private void PatrolMove()
        {
            Vector3 Distance;
            if (_startPosition == Vector3.zero)
            {
                _startPosition = transform.position;
                RandomBorder(false);
            }

            if (_eDetectionArea.GetDetectionArea())
            {
                _patrolDirection = 0;
                Distance = _playerTransform.position - gameObject.transform.position;
                transform.Rotate(Vector3.up, Vector3.Cross(Distance, gameObject.transform.forward).y > 0 ? 180 : 0,
                    Space.Self); //左边
                if (Distance.magnitude < 1.2f)
                {
                    _rb2dEnemy.velocity = Vector3.zero;
                    _eAnimationController.AttackAnimation();
                    return;
                }

                Distance = Distance.normalized;
                _eAnimationController.WalkAnimation();
                _rb2dEnemy.velocity = Distance * baseSpeed;
            }
            else
            {
                switch (_patrolDirection)
                {
                    case -1:
                        Distance = _leftPatrolBorder - gameObject.transform.position;
                        if (Distance.magnitude < endError) _patrolDirection = 1;
                        break;
                    case 1:
                        Distance = _rightPatrolBorder - gameObject.transform.position;
                        if (Distance.magnitude < endError) _patrolDirection = -1;
                        break;
                    default:
                        Distance = _startPosition - gameObject.transform.position;
                        transform.Rotate(Vector3.up,
                            Vector3.Cross(Distance, gameObject.transform.forward).y > 0 ? 180 : 0,
                            Space.Self);
                        Distance = Distance.normalized;
                        _rb2dEnemy.velocity = Distance * baseSpeed;
                        if (Vector3.Distance(_startPosition, gameObject.transform.position) < endError)
                            RandomBorder(true);
                        return;
                }
                transform.Rotate(Vector3.up, Vector3.Cross(Distance, gameObject.transform.forward).y > 0 ? 180 : 0,
                    Space.Self);
                Distance = Distance.normalized;
                _rb2dEnemy.velocity = Distance * baseSpeed;
            }
        }

        private void RandomBorder(bool reset)
        {
            //TODO:需要修改
            var RandomBorderNum = Random.Range(0f, endError);
            _patrolDirection = Random.Range(0, 2) == 0 ? -1 : 1;

            var RandomVector = new Vector3(RandomBorderNum, 0, 0);
            _leftPatrolBorder = _startPosition - transform.right * 2 - RandomVector;
            _rightPatrolBorder = _startPosition + transform.right * 2 + RandomVector;
        }

        #region 属性

        public float baseSpeed = 0.5f;
        public float endError = 0.5f;
        private Vector3 _startPosition;

        private Vector3 _leftPatrolBorder; //左边界
        private Vector3 _rightPatrolBorder; //右边界
        private int _patrolDirection; //巡逻方向

        #endregion

        #region 成员

        public EnemyType enemyType;
        private Collider2D _cr2DEnemy;
        private Rigidbody2D _rb2dEnemy;
        private Transform _playerTransform;
        private SpriteRenderer _spriteRenderer;
        private EnemyAnimationController _eAnimationController;
        private EnemyDetectionArea _eDetectionArea;

        #endregion
    }
}