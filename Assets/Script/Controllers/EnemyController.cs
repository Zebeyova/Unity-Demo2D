using Script.Models;
using Script.RunTimeData;
using Script.Views;
using UnityEngine;

namespace Script.Controllers
{
    public enum EnemyType
    {
        Guard,
        Patrol
    }

    public class EnemyController : MonoBehaviour
    {
        public EnemyType enemyType;
        public LayerMask wallLayerMask;

        private EnemyRunTimeData _runTimeData;
        private EnemyAnimView _animView;

        private Rigidbody2D _rb;
        private GameObject _player;

        private Vector3 _startPos;
        private Vector3 _leftBorder, _rightBorder;
        private int _patrolDir; // -1左, 1右, 0未初始化
        private bool _isTouchingWall;
        private float _wallTimer = 2f;
        private bool _wallTiming;

        // 延迟退出检测
        private bool _playerInTrigger;
        private bool _enterDelay;
        private float _exitDelayTimer = 1f;

        private void Awake()
        {
            _runTimeData = GetComponent<EnemyRunTimeData>();
            _rb = GetComponent<Rigidbody2D>();
            _animView = GetComponent<EnemyAnimView>();
            _player = GameObject.FindWithTag("Player");
            _startPos = transform.position;

            CreateDetectionTrigger();
        }

        private void Start()
        {
            _animView.OnAttackPlayer += OnAttackPlayerHandler;
            _animView.OnAttackEnd += OnAttackEndHandler;
            _animView.OnHurtEnd += OnHurtEndHandler;
            _animView.OnDeathEnd += OnDeathEndHandler;
        }

        private void OnDestroy()
        {
            if (!_animView) return;
            _animView.OnAttackPlayer -= OnAttackPlayerHandler;
            _animView.OnAttackEnd -= OnAttackEndHandler;
            _animView.OnHurtEnd -= OnHurtEndHandler;
            _animView.OnDeathEnd -= OnDeathEndHandler;
        }

        private void Update()
        {
            if (!_player) return;
            UpdateDetectionDelay();
            if (_runTimeData.currentState == EnemyState.Attack || _runTimeData.currentState == EnemyState.Hurt ||
                _runTimeData.currentState == EnemyState.Die) return;
            WallCheck();
            EnemyAI();
        }

        private void EnemyAI()
        {
            switch (enemyType)
            {
                case EnemyType.Guard: GuardBehavior(); break;
                case EnemyType.Patrol: PatrolBehavior(); break;
            }
        }

        #region 触发器检测

        private void CreateDetectionTrigger()
        {
            var triggerObj = new GameObject("DetectionTrigger");
            triggerObj.transform.SetParent(transform);
            triggerObj.transform.localPosition = new Vector3(1, 0.6f, 0);
            var coll = triggerObj.AddComponent<BoxCollider2D>();
            coll.isTrigger = true;
            coll.size = new Vector2(_runTimeData.DetectSizeX, _runTimeData.DetectSizeY);
            var detector = triggerObj.AddComponent<DetectionLogic>();
            detector.OnPlayerEnter += () =>
            {
                _playerInTrigger = true;
                _enterDelay = true;
            };
            detector.OnPlayerExit += () =>
            {
                _enterDelay = false;
                _exitDelayTimer = 1f;
            };
        }

        private void UpdateDetectionDelay()
        {
            if (_enterDelay) return;
            _exitDelayTimer -= Time.deltaTime;
            if (!(_exitDelayTimer <= 0)) return;
            _playerInTrigger = false;
        }

        private bool IsPlayerDetected() => _playerInTrigger;

        #endregion

        #region Guard 逻辑

        private void GuardBehavior()
        {
            var playerDetected = IsPlayerDetected();

            if (playerDetected && !_isTouchingWall)
                AttackOrMove(_player.transform.position - transform.position);
            else
            {
                // 返回起始点
                var dir = _startPos - transform.position;
                if (dir.magnitude < _runTimeData.EndError)
                {
                    StopMoveAndIdle();
                    _isTouchingWall = false;
                    return;
                }

                Move(dir);
            }
        }

        #endregion

        #region Patrol 逻辑

        private void PatrolBehavior()
        {
            var playerDetected = IsPlayerDetected();

            if (playerDetected && !_isTouchingWall)
            {
                _patrolDir = 0; // 停止巡逻，转为追击
                AttackOrMove(_player.transform.position - transform.position);
            }
            else
            {
                if (_patrolDir == 0) RandomBorder(); // 首次初始化边界

                var target = _patrolDir switch
                {
                    -1 => _leftBorder,
                    1 => _rightBorder,
                    _ => _startPos
                };

                var dir = target - transform.position;
                if (dir.magnitude < _runTimeData.EndError)
                {
                    if (_patrolDir == 0)
                        RandomBorder(); // 第一次到达起始点后随机边界
                    else
                        _patrolDir = -_patrolDir; // 掉头
                }

                Move(dir);
            }
        }

        private void RandomBorder()
        {
            var offset = Random.Range(0f, _runTimeData.EndError);
            _patrolDir = Random.Range(0, 2) == 0 ? -1 : 1;
            _leftBorder = _startPos - transform.right * _runTimeData.PatrolMaxDistance - new Vector3(offset, 0, 0);
            _rightBorder = _startPos + transform.right * _runTimeData.PatrolMaxDistance + new Vector3(offset, 0, 0);
        }

        #endregion

        private void AttackOrMove(Vector3 direction)
        {
            if (direction.magnitude < _runTimeData.DistanceFromPlayer)
            {
                // 停止移动，播放攻击动画
                _rb.velocity = Vector2.zero;
                _runTimeData.currentState = EnemyState.Attack;
            }
            else Move(direction);
        }

        private void Move(Vector3 direction)
        {
            var moveDir = new Vector2(direction.x, 0).normalized;
            if (moveDir.x != 0)
            {
                var shouldFaceRight = moveDir.x > 0;
                if ((shouldFaceRight && transform.localScale.x < 0) || (!shouldFaceRight && transform.localScale.x > 0))
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y,
                        transform.localScale.z);
                }
            }

            _runTimeData.currentState = EnemyState.Walk;
            _rb.velocity = moveDir.normalized * _runTimeData.BaseSpeed;
        }

        private void StopMoveAndIdle()
        {
            _rb.velocity = Vector2.zero;
            _runTimeData.currentState = EnemyState.Idle;
        }

        private void WallCheck()
        {
            var origin = (Vector2)transform.position + Vector2.up * 0.5f;
            var hit = Physics2D.Raycast(origin, transform.right, 1f, wallLayerMask);

            if (!hit && !_wallTiming) return;

            _wallTiming = true;
            _rb.velocity = Vector2.zero;
            _runTimeData.currentState = EnemyState.Idle;
            _wallTimer -= Time.deltaTime;

            if (!(_wallTimer <= 0)) return;
            _wallTimer = 2f;
            _isTouchingWall = true;
            _wallTiming = false;
        }

        #region 动画事件回调

        private void OnAttackPlayerHandler()
        {
            if (IsPlayerDetected() &&
                (_player.transform.position - transform.position).magnitude < _runTimeData.DistanceFromPlayer &&
                !_isTouchingWall) _runTimeData.AttackPlayer(_player);
            else OnAttackEndHandler();
        }

        private void OnAttackEndHandler()
        {
            if (_runTimeData.currentState != EnemyState.Attack) return;
            // 攻击结束：根据是否检测到玩家及是否在冷却来决定状态
            if (IsPlayerDetected() && !_isTouchingWall)
            {
                // 继续追击或再次攻击
                AttackOrMove(_player.transform.position - transform.position);
            }
            else
            {
                if (enemyType == EnemyType.Guard)
                {
                    var toStart = _startPos - transform.position;
                    if (toStart.magnitude > _runTimeData.EndError)
                        Move(toStart);
                    else
                        StopMoveAndIdle();
                }
                else // Patrol
                {
                    if (_patrolDir == 0) RandomBorder();
                    StopMoveAndIdle();
                }
            }
        }

        private void OnHurtEndHandler()
        {
            if (_runTimeData.currentState != EnemyState.Hurt) return;
            if (IsPlayerDetected() && !_isTouchingWall)
                AttackOrMove(_player.transform.position - transform.position);
            else StopMoveAndIdle();
        }

        private void OnDeathEndHandler()
        {
            if (_runTimeData.currentState != EnemyState.Die) return;
            Destroy(gameObject);
        }

        #endregion

        // 嵌套触发器类
        private class DetectionLogic : MonoBehaviour
        {
            public event System.Action OnPlayerEnter;
            public event System.Action OnPlayerExit;

            private void OnTriggerEnter2D(Collider2D other)
            {
                if (other.CompareTag("Player")) OnPlayerEnter?.Invoke();
            }

            private void OnTriggerExit2D(Collider2D other)
            {
                if (other.CompareTag("Player")) OnPlayerExit?.Invoke();
            }
        }
    }
}