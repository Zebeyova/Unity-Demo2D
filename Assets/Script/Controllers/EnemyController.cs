using Script.Interfaces;
using Script.RunTimeDatas;
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

        private void Awake()
        {
            _runTimeData = GetComponent<EnemyRunTimeData>();
            _rb = GetComponent<Rigidbody2D>();
            GetComponent<SpriteRenderer>();
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
            if (_runTimeData.currentStats == IDamageable.Stats.Attack ||
                _runTimeData.currentStats == IDamageable.Stats.Hurt ||
                _runTimeData.currentStats == IDamageable.Stats.Death) return;
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
            triggerObj.transform.localPosition = new Vector3(0.3f, 0.6f, 0);
            var coll = triggerObj.AddComponent<BoxCollider2D>();
            coll.isTrigger = true;
            coll.size = new Vector2(3f, 1.2f);
            var detector = triggerObj.AddComponent<DetectionLogic>();
            detector.OnPlayerEnter += () => { _playerInTrigger = true; };
            detector.OnPlayerExit += () => { _playerInTrigger = false; };
        }

        private bool IsPlayerDetected()
        {
            return _playerInTrigger;
        }

        #endregion

        #region Guard 逻辑

        private void GuardBehavior()
        {
            var playerDetected = IsPlayerDetected();

            if (playerDetected && !_isTouchingWall)
            {
                TryAttackOrMove(_player.transform.position - transform.position);
            }
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
                TryAttackOrMove(_player.transform.position - transform.position);
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
                    _isTouchingWall = false;
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

        private void TryAttackOrMove(Vector3 direction)
        {
            if (direction.magnitude < _runTimeData.DistanceFromPlayer)
            {
                // 停止移动，播放攻击动画
                StopMoveAndIdle();
                _runTimeData.currentStats = IDamageable.Stats.Attack;
            }
            else
            {
                Move(direction);
            }
        }

        private void Move(Vector3 direction)
        {
            var moveDir = new Vector2(direction.x, 0).normalized;
            if (moveDir.x != 0) transform.eulerAngles = new Vector3(0, moveDir.x < 0 ? 180 : 0, 0);

            _runTimeData.currentStats = IDamageable.Stats.Walk;
            _rb.velocity = moveDir.normalized * _runTimeData.BaseSpeed;
        }

        private void StopMoveAndIdle()
        {
            _rb.velocity = Vector2.zero;
            _runTimeData.currentStats = IDamageable.Stats.Idle;
        }

        private void WallCheck()
        {
            var origin = (Vector2)transform.position + Vector2.up * 0.5f;
            var hit = Physics2D.Raycast(origin, transform.right + transform.up, 1f, wallLayerMask);
            if (!hit && !_wallTiming) return;

            _wallTiming = true;
            _rb.velocity = Vector2.zero;
            _runTimeData.currentStats = IDamageable.Stats.Idle;
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
                !_isTouchingWall)
                Events.EventCenter.TriggerAttackHit(new Events.AttackEventArgs
                {
                    attacker = gameObject,
                    target = _player,
                    attackType = IDamageable.Stats.Attack
                });
            else OnAttackEndHandler();
        }

        private void OnAttackEndHandler()
        {
            if (_runTimeData.currentStats != IDamageable.Stats.Attack) return;
            // 攻击结束：根据是否检测到玩家及是否在冷却来决定状态
            if (IsPlayerDetected() && !_isTouchingWall)
            {
                // 继续追击或再次攻击
                TryAttackOrMove(_player.transform.position - transform.position);
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
            if (_runTimeData.currentStats != IDamageable.Stats.Hurt) return;
            if (IsPlayerDetected() && !_isTouchingWall)
                TryAttackOrMove(_player.transform.position - transform.position);
            else StopMoveAndIdle();
        }

        private void OnDeathEndHandler()
        {
            if (_runTimeData.currentStats != IDamageable.Stats.Death) return;
            Destroy(gameObject.transform.parent.gameObject);
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