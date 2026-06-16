using System.Collections;
using Script.Interfaces;
using Script.RunTimeDatas;
using Script.Views;
using UnityEngine;

namespace Script.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [Header("碰撞层")] public LayerMask groundLayerMask;

        private PlayerRunTimeData _runTimeData;
        private PlayerAnimView _animView;
        private Rigidbody2D _rb;
        private Collider2D _collider;

        // 输入标志
        private float _horizontal;
        private bool _inGround;
        private bool _isWalking, _isRunning, _isAttacking, _isSliding;

        private bool _isSlidingOnCooldown;

        // 转向
        private bool _currentFacingRight = true;
        private bool _targetFacingRight;
        private bool? _pendingTurnFacingRight;

        private void Awake()
        {
            _runTimeData = GetComponent<PlayerRunTimeData>();
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _animView = GetComponent<PlayerAnimView>();
        }

        private void Start()
        {
            _animView.OnJumpPeak += OnJumpPeakHandler;
            _animView.OnLanding += OnLandingHandler;
            _animView.OnTurnEnd += OnTurnEndHandler;
            _animView.OnSlideEnd += OnSlideEndHandler;
            _animView.OnAttackHit += OnAttackHitHandler;
            _animView.OnAttackEnd += OnAttackEndHandler;
            _animView.OnHurtEnd += OnHurtEndHandler;
        }

        private void OnDestroy()
        {
            if (!_animView) return;
            _animView.OnJumpPeak -= OnJumpPeakHandler;
            _animView.OnLanding -= OnLandingHandler;
            _animView.OnTurnEnd -= OnTurnEndHandler;
            _animView.OnSlideEnd -= OnSlideEndHandler;
            _animView.OnAttackHit -= OnAttackHitHandler;
            _animView.OnAttackEnd -= OnAttackEndHandler;
            _animView.OnHurtEnd -= OnHurtEndHandler;
        }

        private void Update()
        {
            _inGround = _collider.IsTouchingLayers(groundLayerMask);
            if (_runTimeData.currentState == IDamageable.Stats.Hurt ||
                _runTimeData.currentState == IDamageable.Stats.Death) return;
            HandleInput();
            HandleTurn();
            Move();
        }

        private void HandleInput()
        {
            _horizontal = Input.GetAxis("Horizontal");
            _isWalking = _isRunning = _isSliding = false;

            // 跳跃
            if (Input.GetKeyDown(KeyCode.K) && _inGround)
            {
                _runTimeData.currentState = IDamageable.Stats.Jump;
                _rb.velocity = new Vector2(_rb.velocity.x, _runTimeData.JumpForce);
                return;
            }

            // 滑铲
            if (Input.GetKeyDown(KeyCode.Space) && _runTimeData.currentState == IDamageable.Stats.Run &&
                _inGround &&
                !_isSlidingOnCooldown)
            {
                _runTimeData.currentState = IDamageable.Stats.Slide;
                _isSliding = true;
                StartCoroutine(SlideCooldownRoutine());
                return;
            }

            // 技能攻击
            if (Input.GetKeyDown(KeyCode.L))
            {
                _runTimeData.currentState = IDamageable.Stats.Skill;
                _isAttacking = true;
                return;
            }

            // 普通攻击
            if (Input.GetKeyDown(KeyCode.J))
            {
                switch (_runTimeData.currentState)
                {
                    case IDamageable.Stats.Hurt:
                    case IDamageable.Stats.Death:
                        return;
                    case IDamageable.Stats.Attack:
                        _runTimeData.currentState = IDamageable.Stats.Attack2;
                        _isAttacking = true;
                        return;
                }

                if (_runTimeData.currentState != IDamageable.Stats.Attack2)
                {
                    _runTimeData.currentState = IDamageable.Stats.Attack;
                    _isAttacking = true;
                    return;
                }
            }

            var isSpecialState = _runTimeData.currentState == IDamageable.Stats.Jump ||
                                 _runTimeData.currentState == IDamageable.Stats.Fall ||
                                 _runTimeData.currentState == IDamageable.Stats.FallLoop ||
                                 _runTimeData.currentState == IDamageable.Stats.WalkTurn ||
                                 _runTimeData.currentState == IDamageable.Stats.RunTurn ||
                                 _runTimeData.currentState == IDamageable.Stats.Slide ||
                                 _runTimeData.currentState == IDamageable.Stats.Attack ||
                                 _runTimeData.currentState == IDamageable.Stats.Attack2 ||
                                 _runTimeData.currentState == IDamageable.Stats.Skill ||
                                 _runTimeData.currentState == IDamageable.Stats.Hurt ||
                                 _runTimeData.currentState == IDamageable.Stats.Death;

            var _wantMove = Mathf.Abs(_horizontal) > _runTimeData.HorizontalInputThreshold;
            if (isSpecialState)
            {
                // 特殊状态下仍记录移动输入，用于空中移动速度
                if (!(_wantMove)) return;
                if (Input.GetKey(KeyCode.LeftShift))
                    _isRunning = true;
                else
                    _isWalking = true;
            }
            else
            {
                // 奔跑
                if (Input.GetKey(KeyCode.LeftShift) && _wantMove)
                {
                    _runTimeData.currentState = IDamageable.Stats.Run;
                    _isRunning = true;
                }
                // 移动
                else if (_wantMove)
                {
                    _runTimeData.currentState = IDamageable.Stats.Walk;
                    _isWalking = true;
                }
                else
                {
                    _runTimeData.currentState = IDamageable.Stats.Idle;
                }
            }
        }

        private IEnumerator SlideCooldownRoutine()
        {
            _isSlidingOnCooldown = true;
            yield return new WaitForSeconds(_runTimeData.SlideCool);
            _isSlidingOnCooldown = false;
        }

        private void Move()
        {
            if (_isAttacking)
            {
                _rb.velocity = Vector2.zero;
                _rb.gravityScale = 0;
                return;
            }

            var speed = _runTimeData.BaseSpeed;
            if (_isSliding) speed *= _runTimeData.RunSpeedMultiplier * _runTimeData.SlideSpeedMultiplier;
            else if (_isRunning) speed *= _runTimeData.RunSpeedMultiplier;

            if (_isWalking || _isRunning || _isSliding)
                _rb.velocity = new Vector2(_horizontal * speed, _rb.velocity.y);
        }

        private void HandleTurn()
        {
            _targetFacingRight = _horizontal > _runTimeData.HorizontalInputThreshold;
            if (_currentFacingRight == _targetFacingRight ||
                Mathf.Abs(_horizontal) <= _runTimeData.HorizontalInputThreshold)
            {
                _pendingTurnFacingRight = null;
                return;
            }

            if (_inGround && (_runTimeData.currentState == IDamageable.Stats.Walk ||
                              _runTimeData.currentState == IDamageable.Stats.Run)) //地面转身
            {
                _runTimeData.currentState = _runTimeData.currentState == IDamageable.Stats.Walk
                    ? IDamageable.Stats.WalkTurn
                    : IDamageable.Stats.RunTurn;
                _pendingTurnFacingRight = _targetFacingRight;
            }
            else if (_runTimeData.currentState == IDamageable.Stats.Jump ||
                     _runTimeData.currentState == IDamageable.Stats.Fall) //空中转身
            {
                _currentFacingRight = _targetFacingRight;
                transform.localRotation = Quaternion.Euler(0, _currentFacingRight ? 0 : 180, 0);
                _pendingTurnFacingRight = null;
            }
        }

        private void OnJumpPeakHandler()
        {
            if (_runTimeData.currentState != IDamageable.Stats.Jump) return;
            if (!_inGround) _runTimeData.currentState = IDamageable.Stats.Fall;
            else
            {
                if (_isRunning)
                    _runTimeData.currentState = IDamageable.Stats.Run;
                else if (_isWalking)
                    _runTimeData.currentState = IDamageable.Stats.Walk;
                else
                    _runTimeData.currentState = IDamageable.Stats.Idle;
            }
        }

        private void OnLandingHandler()
        {
            if (_runTimeData.currentState != IDamageable.Stats.Fall &&
                _runTimeData.currentState != IDamageable.Stats.FallLoop) return;
            if (!_inGround) _runTimeData.currentState = IDamageable.Stats.FallLoop;
            else
            {
                if (_isRunning)
                    _runTimeData.currentState = IDamageable.Stats.Run;
                else if (_isWalking)
                    _runTimeData.currentState = IDamageable.Stats.Walk;
                else
                    _runTimeData.currentState = IDamageable.Stats.Idle;
            }
        }

        private void OnTurnEndHandler()
        {
            if (_runTimeData.currentState != IDamageable.Stats.WalkTurn &&
                _runTimeData.currentState != IDamageable.Stats.RunTurn)
                return;

            // 应用待处理的转身
            if (_pendingTurnFacingRight.HasValue)
            {
                _currentFacingRight = _pendingTurnFacingRight.Value;
                transform.localRotation = Quaternion.Euler(0, _currentFacingRight ? 0 : 180, 0);
                _pendingTurnFacingRight = null;
            }

            // 根据输入和地面状态恢复状态
            var wantsWalk = Mathf.Abs(_horizontal) > _runTimeData.HorizontalInputThreshold;
            var wantsRun = Input.GetKey(KeyCode.LeftShift) && wantsWalk;
            if (_inGround)
            {
                if (wantsRun)
                    _runTimeData.currentState = IDamageable.Stats.Run;
                else if (wantsWalk)
                    _runTimeData.currentState = IDamageable.Stats.Walk;
                else
                    _runTimeData.currentState = IDamageable.Stats.Idle;
            }
            else
            {
                _runTimeData.currentState = IDamageable.Stats.Fall;
            }
        }

        private void OnSlideEndHandler()
        {
            if (_runTimeData.currentState != IDamageable.Stats.Slide) return;
            _isSliding = false;
            _runTimeData.currentState = _inGround ? IDamageable.Stats.Idle : IDamageable.Stats.Fall;
        }

        private void OnAttackHitHandler()
        {
            var state = _runTimeData.currentState;
            if (state != IDamageable.Stats.Attack && state != IDamageable.Stats.Attack2 &&
                state != IDamageable.Stats.Skill) return;

            var hit = Physics2D.Raycast(_collider.bounds.center,
                _currentFacingRight ? Vector2.right : Vector2.left,
                1f, LayerMask.GetMask("Enemy"));
            if (hit.collider && hit.collider.CompareTag("Enemy"))
            {
                Events.EventCenter.TriggerAttackHit(new Events.AttackEventArgs
                {
                    attacker = gameObject,
                    target = hit.collider.gameObject,
                    attackType = state
                });
            }
        }

        private void OnAttackEndHandler()
        {
            if (_runTimeData.currentState != IDamageable.Stats.Attack &&
                _runTimeData.currentState != IDamageable.Stats.Attack2 &&
                _runTimeData.currentState != IDamageable.Stats.Skill) return;
            _runTimeData.currentState = _inGround ? IDamageable.Stats.Idle : IDamageable.Stats.Fall;
            _isAttacking = false;
            _rb.gravityScale = 2;
        }

        private void OnHurtEndHandler()
        {
            if (_runTimeData.currentState != IDamageable.Stats.Hurt) return;
            var wantsWalk = Mathf.Abs(_horizontal) > _runTimeData.HorizontalInputThreshold;
            var wantsRun = Input.GetKey(KeyCode.LeftShift) && wantsWalk;
            if (wantsRun)
                _runTimeData.currentState = IDamageable.Stats.Run;
            else if (wantsWalk)
                _runTimeData.currentState = IDamageable.Stats.Walk;
            else
                _runTimeData.currentState = IDamageable.Stats.Idle;
            _isAttacking = false;
            _rb.gravityScale = 2;
        }
    }
}