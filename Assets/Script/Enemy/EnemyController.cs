using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
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

        private void Update()
        {
            EnemyControl();
            WallCheck();
        }

        private void CheckComponent()
        {
            _rb2dEnemy = GetComponent<Rigidbody2D>();
            _eAnimationController = GetComponent<EnemyAnimationController>();
            _eDetectionArea = GetComponentInChildren<EnemyDetectionArea>();
            _enemyProperties = FindObjectOfType<EnemyProperties>();
            _health = GetComponent<Health>();
        }

        private void EnemyControl()
        {
            if (_health.currentHealth == 0) _eAnimationController.HurtAnimation(0, 0);
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
            var Distance = new Vector3(_playerTransform.position.x - gameObject.transform.position.x, 0, 0);
            if (_startPosition == Vector3.zero) _startPosition = transform.position;
            if (_eDetectionArea.GetDetectionArea() && !_isTouchWall) //追击状态
            {
                if (_isAttackingCooldown)
                {
                    _rb2dEnemy.velocity = Vector3.zero;
                    _eAnimationController.IdleAnimation();
                    return;
                }

                AttackOperation(Distance);
            }
            else //返回守卫状态
            {
                Distance = (_startPosition - gameObject.transform.position).normalized;

                if (Vector3.Distance(_startPosition, gameObject.transform.position) < _enemyProperties.endError)
                {
                    _rb2dEnemy.velocity = Vector3.zero;
                    _eAnimationController.IdleAnimation();
                    _isTouchWall = false;
                    return;
                }

                EnemyMove(Distance);
            }
        }

        private void PatrolMove()
        {
            if (_startPosition == Vector3.zero)
            {
                _startPosition = transform.position;
                RandomBorder();
            }

            var Distance = new Vector3(_playerTransform.position.x - gameObject.transform.position.x, 0, 0);
            if (_eDetectionArea.GetDetectionArea() && !_isTouchWall) //准备攻击
            {
                _patrolDirection = 0;
                if (_isAttackingCooldown)
                {
                    _rb2dEnemy.velocity = Vector3.zero;
                    _eAnimationController.IdleAnimation();
                    return;
                }

                AttackOperation(Distance);
            }
            else
            {
                switch (_patrolDirection) //巡逻
                {
                    case -1:
                        Distance = _leftPatrolBorder - gameObject.transform.position;
                        if (Distance.magnitude < _enemyProperties.endError) _patrolDirection = 1;
                        break;
                    case 1:
                        Distance = _rightPatrolBorder - gameObject.transform.position;
                        if (Distance.magnitude < _enemyProperties.endError) _patrolDirection = -1;
                        break;
                    default:
                        Distance = _startPosition - gameObject.transform.position;
                        EnemyMove(Distance);
                        if (Vector3.Distance(_startPosition, gameObject.transform.position) < _enemyProperties.endError)
                        {
                            RandomBorder();
                            _isTouchWall = false;
                        }

                        return;
                }

                EnemyMove(Distance);
            }
        }

        private void EnemyMove(Vector3 distance)
        {
            transform.Rotate(Vector3.up, Vector3.Cross(distance, gameObject.transform.forward).y > 0 ? 180 : 0,
                Space.Self); //旋转
            _eAnimationController.WalkAnimation();
            _rb2dEnemy.velocity = distance.normalized * _enemyProperties.baseSpeed;
        }

        private void WallCheck()
        {
            if (!_startTiming && !Physics2D.Raycast(transform.position + new Vector3(0, 0.5f, 0),
                    transform.right + Vector3.up, 1f, wallLayerMask)) return;
            _startTiming = true;
            _rb2dEnemy.velocity = Vector3.zero;
            _eAnimationController.IdleAnimation();
            _timer -= Time.unscaledDeltaTime;
            if (!(_timer <= 0)) return;
            _timer = 2f;
            _isTouchWall = true;
            _startTiming = false;
        }

        private void RandomBorder()
        {
            var RandomBorderNum = Random.Range(0f, _enemyProperties.endError);
            _patrolDirection = Random.Range(0, 2) == 0 ? -1 : 1;

            var RandomVector = new Vector3(RandomBorderNum, 0, 0);
            _leftPatrolBorder = _startPosition - transform.right * 2 - RandomVector;
            _rightPatrolBorder = _startPosition + transform.right * 2 + RandomVector;
        }

        private void AttackOperation(Vector3 distance)
        {
            if (distance.magnitude < _enemyProperties.distanceFromPlayer && !_allowAttack)
            {
                _rb2dEnemy.velocity = Vector3.zero;
                _eAnimationController.AttackAnimation();
                _isAttackingCooldown = true;
                if (_attackCooldownCoroutine != null) StopCoroutine(_attackCooldownCoroutine);
                _attackCooldownCoroutine = StartCoroutine(AttackCooldown());
                return;
            }

            EnemyMove(distance);
        }

        private IEnumerator AttackCooldown()
        {
            _allowAttack = true;
            yield return new WaitForSeconds(_enemyProperties.attackCoolDown);
            _isAttackingCooldown = false;
            _allowAttack = false;
        }

        public void DestroyEnemy()
        {
            Destroy(gameObject);
        }

        #region 属性

        public LayerMask wallLayerMask;
        private Vector3 _startPosition; //起始点
        private Vector3 _leftPatrolBorder; //左边界
        private Vector3 _rightPatrolBorder; //右边界
        private int _patrolDirection; //巡逻方向
        private bool _isTouchWall; //是否碰到墙壁
        private float _timer = 2f;
        private bool _startTiming; //开始计时
        private bool _allowAttack; //允许攻击
        private bool _isAttackingCooldown; //攻击冷却中

        #endregion

        #region 成员

        public EnemyType enemyType;
        private Rigidbody2D _rb2dEnemy;
        private Transform _playerTransform;
        private EnemyAnimationController _eAnimationController;
        private EnemyDetectionArea _eDetectionArea;
        private EnemyProperties _enemyProperties;
        private Coroutine _attackCooldownCoroutine; //攻击冷却协程
        private Health _health;

        #endregion
    }
}