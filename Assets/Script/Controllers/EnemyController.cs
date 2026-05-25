using System.Collections;
using Script.Models;
using Script.RunTimeData;
using Script.Views;
using UnityEngine;

namespace Script.Controllers
{
    public enum EnemyType { Guard, Patrol }

    public class EnemyController : MonoBehaviour
    {
        public EnemyType enemyType;
        public LayerMask wallLayerMask;
        [Header("Detection")]
        public float detectRadius = 5f;

        private EnemyRunTimeData _data;
        private Rigidbody2D _rb;
        private Transform _player;

        private Vector3 _startPos;
        private Vector3 _leftBorder, _rightBorder;
        private int _patrolDir;
        private bool _isTouchingWall;
        private float _wallTimer = 2f;
        private bool _wallTiming;
        private bool _attackCooldown;
        private Coroutine _attackCooldownCoroutine;

        // 延迟退出检测相关
        private bool _playerInTrigger;
        private bool _exitDelay;
        private float _exitDelayTimer = 1f;

        private void Awake()
        {
            _data = GetComponent<EnemyRunTimeData>();
            _rb = GetComponent<Rigidbody2D>();
            GetComponent<EnemyAnimView>();
            _player = GameObject.FindWithTag("Player")?.transform;
            _startPos = transform.position;

            // 自动创建触发器子物体
            CreateDetectionTrigger();
        }

        private void CreateDetectionTrigger()
        {
            var triggerObj = new GameObject("DetectionTrigger");
            triggerObj.transform.SetParent(transform);
            triggerObj.transform.localPosition = Vector3.zero;
            var coll = triggerObj.AddComponent<CircleCollider2D>();
            coll.isTrigger = true;
            coll.radius = detectRadius;
            var detector = triggerObj.AddComponent<DetectionLogic>();
            detector.OnPlayerEnter += () =>
            {
                _playerInTrigger = true;
                _exitDelay = false;
            };
            detector.OnPlayerExit += () =>
            {
                _exitDelay = true;
                _exitDelayTimer = 1f;
            };
        }

        private void Update()
        {
            if (!_player) return;
            UpdateDetection();
            EnemyAI();
            WallCheck();
        }

        private void UpdateDetection()
        {
            if (!_exitDelay) return;
            _exitDelayTimer -= Time.deltaTime;
            if (!(_exitDelayTimer <= 0)) return;
            _playerInTrigger = false;
            _exitDelay = false;
        }

        private bool IsPlayerDetected() => _playerInTrigger;

        private void EnemyAI()
        {
            switch (enemyType)
            {
                case EnemyType.Guard: GuardBehavior(); break;
                case EnemyType.Patrol: PatrolBehavior(); break;
            }
        }

        private void GuardBehavior()
        {
            var toPlayer = _player.position - transform.position;
            var playerDetected = IsPlayerDetected();

            if (playerDetected && !_isTouchingWall)
            {
                if (_attackCooldown)
                {
                    _rb.velocity = Vector2.zero;
                    _data.currentState = EnemyState.Idle;
                    return;
                }
                AttackOrMove(toPlayer);
            }
            else
            {
                var backToStart = (_startPos - transform.position).normalized;
                if (Vector3.Distance(_startPos, transform.position) < _data.EndError)
                {
                    _rb.velocity = Vector2.zero;
                    _data.currentState = EnemyState.Idle;
                    _isTouchingWall = false;
                    return;
                }
                Move(backToStart);
            }
        }

        private void PatrolBehavior()
        {
            if (_startPos == Vector3.zero) _startPos = transform.position;
            var playerDetected = IsPlayerDetected();

            if (playerDetected && !_isTouchingWall)
            {
                _patrolDir = 0;
                if (_attackCooldown)
                {
                    _rb.velocity = Vector2.zero;
                    _data.currentState = EnemyState.Idle;
                    return;
                }
                AttackOrMove(_player.position - transform.position);
            }
            else
            {
                if (_patrolDir == 0) InitPatrolBorder();
                Vector3 target;
                if (_patrolDir == -1)
                    target = _leftBorder;
                else if (_patrolDir == 1)
                    target = _rightBorder;
                else
                    target = _startPos;

                var dir = target - transform.position;
                if (dir.magnitude < _data.EndError)
                {
                    if (_patrolDir == 0)
                        RandomBorder();
                    else
                        _patrolDir = -_patrolDir;
                }
                Move(dir);
            }
        }

        private void AttackOrMove(Vector3 direction)
        {
            if (direction.magnitude < _data.DistanceFromPlayer)
            {
                _rb.velocity = Vector2.zero;
                _data.currentState = EnemyState.Attack;
                if (_attackCooldownCoroutine != null) StopCoroutine(_attackCooldownCoroutine);
                _attackCooldownCoroutine = StartCoroutine(AttackCooldownRoutine());
            }
            else
            {
                Move(direction);
            }
        }

        private void Move(Vector3 direction)
        {
            // 转向
            var dot = Vector3.Dot(direction, transform.right);
            if (dot < 0) // 如果方向与当前朝向相反，则翻转
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }

            _data.currentState = EnemyState.Walk;
            _rb.velocity = direction.normalized * _data.BaseSpeed;
        }

        private void WallCheck()
        {
            var origin = (Vector2)transform.position + Vector2.up * 0.5f;
            var hit = Physics2D.Raycast(origin, transform.right, 1f, wallLayerMask);
            if (!hit && !_wallTiming) return;

            _wallTiming = true;
            _rb.velocity = Vector2.zero;
            _data.currentState = EnemyState.Idle;
            _wallTimer -= Time.deltaTime;
            if (!(_wallTimer <= 0)) return;
            _wallTimer = 2f;
            _isTouchingWall = true;
            _wallTiming = false;
        }

        private void InitPatrolBorder() => RandomBorder();

        private void RandomBorder()
        {
            var offset = Random.Range(0f, _data.EndError);
            _patrolDir = Random.Range(0, 2) == 0 ? -1 : 1;
            _leftBorder = _startPos - transform.right * _data.PatrolMaxDistance - new Vector3(offset, 0, 0);
            _rightBorder = _startPos + transform.right * _data.PatrolMaxDistance + new Vector3(offset, 0, 0);
        }

        private IEnumerator AttackCooldownRoutine()
        {
            _attackCooldown = true;
            yield return new WaitForSeconds(_data.AttackCoolDown);
            _attackCooldown = false;
        }

        // 嵌套触发器逻辑类
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