using Script.Models;
using Script.RunTimeData;
using Script.Views;
using UnityEngine;

namespace Script.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [Header("碰撞层")] public LayerMask groundLayerMask;

        private PlayerRunTimeData _runTimeData;
        private Rigidbody2D _rb;
        private Collider2D _collider;

        // 输入标志
        private float _horizontal;
        private bool _inGround;
        private bool _isWalking, _isRunning, _isAttacking, _isSliding;
        private bool _isSlidingOnCooldown;
        private float _slideTimer;

        // 转向
        private bool _currentFacingRight = true;
        private bool _targetFacingRight;
        private bool? _pendingTurnFacingRight;

        private void Awake()
        {
            _runTimeData = GetComponent<PlayerRunTimeData>();
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            GetComponent<PlayerAnimView>();
        }

        private void Start()
        {
            var animView = GetComponent<PlayerAnimView>();
            animView.OnJumpPeak += OnJumpPeakHandler;
            animView.OnLanding += OnLandingHandler;
            animView.OnTurnEnd += OnTurnEndHandler;
        }

        private void Update()
        {
            _inGround = _collider.IsTouchingLayers(groundLayerMask);
            HandleInput();
            HandleTurn();
            Move();
        }

        private void FixedUpdate()
        {
            UpdateSlideCooldown();
        }

        private void HandleInput()
        {
            _horizontal = Input.GetAxis("Horizontal");
            _isWalking = _isRunning = _isSliding = _isAttacking = false;

            // 滑铲
            //TODO:滑铲冷却消失
            if (Input.GetKeyDown(KeyCode.Space) && _runTimeData.currentState == PlayerStats.Run && _inGround &&
                !_isSlidingOnCooldown)
            {
                _runTimeData.currentState = PlayerStats.Slide;
                _isSliding = true;
                Invoke(nameof(EndSlide), 0.4f); // 结束滑铲状态
                return;
            }

            // 跳跃
            if (Input.GetKeyDown(KeyCode.K) && _inGround)
            {
                _runTimeData.currentState = PlayerStats.Jump;
                _rb.velocity = new Vector2(_rb.velocity.x, _runTimeData.JumpForce);
                return;
            }

            // 技能攻击
            if (Input.GetKeyDown(KeyCode.L))
            {
                _runTimeData.currentState = PlayerStats.Skills;
                _isAttacking = true;
                TryAttack(_runTimeData.currentState);
                Invoke(nameof(EndAttack), 0.5f); // 结束攻击状态
                return;
            }

            // 普通攻击
            if (Input.GetKeyDown(KeyCode.J))
            {
                _isAttacking = true;
                if (_runTimeData.comboCount > 3) // 二段连击阈值
                {
                    _runTimeData.currentState = PlayerStats.Attack2;
                    _runTimeData.comboCount = 0;
                }
                else
                {
                    _runTimeData.currentState = PlayerStats.Attack1;
                    _runTimeData.comboCount++;
                }

                TryAttack(_runTimeData.currentState);
                Invoke(nameof(EndAttack), 0.3f); // 结束攻击状态
                return;
            }

            var isSpecialState = _runTimeData.currentState == PlayerStats.Jump ||
                                 _runTimeData.currentState == PlayerStats.Fall ||
                                 _runTimeData.currentState == PlayerStats.WalkTurn ||
                                 _runTimeData.currentState == PlayerStats.RunTurn ||
                                 _runTimeData.currentState == PlayerStats.Slide ||
                                 _runTimeData.currentState == PlayerStats.Attack1 ||
                                 _runTimeData.currentState == PlayerStats.Attack2 ||
                                 _runTimeData.currentState == PlayerStats.Skills ||
                                 _runTimeData.currentState == PlayerStats.Hurt ||
                                 _runTimeData.currentState == PlayerStats.Death;

            if (!isSpecialState)
            {
                // 奔跑
                if (Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(_horizontal) > 0.1f)
                {
                    _runTimeData.currentState = PlayerStats.Run;
                    _isRunning = true;
                }
                // 普通移动
                else if (Mathf.Abs(_horizontal) > _runTimeData.HorizontalInputThreshold)
                {
                    _runTimeData.currentState = PlayerStats.Walk;
                    _isWalking = true;
                }
                else
                {
                    _runTimeData.currentState = PlayerStats.Idle;
                }
            }
            else
            {
                // 特殊状态下仍记录移动输入，用于空中移动速度
                if (!(Mathf.Abs(_horizontal) > _runTimeData.HorizontalInputThreshold)) return;
                if (Input.GetKey(KeyCode.LeftShift))
                    _isRunning = true;
                else
                    _isWalking = true;
            }
        }

        // 结束滑铲状态
        private void EndSlide()
        {
            if (_runTimeData.currentState != PlayerStats.Slide) return;
            _runTimeData.currentState = _inGround ? PlayerStats.Idle : PlayerStats.Fall;
            _isSliding = false;
        }

        // 结束攻击状态
        private void EndAttack()
        {
            if (_runTimeData.currentState != PlayerStats.Attack1 && _runTimeData.currentState != PlayerStats.Attack2 &&
                _runTimeData.currentState != PlayerStats.Skills) return;
            _runTimeData.currentState = _inGround ? PlayerStats.Idle : PlayerStats.Fall;
            _isAttacking = false;
        }

        private void Move()
        {
            if (_isAttacking)
            {
                _rb.velocity = Vector2.zero;
                return;
            }

            var speed = _runTimeData.BaseSpeed;
            if (_isSliding) speed *= _runTimeData.RunSpeedMultiplier * 1.35f;
            else if (_isRunning) speed *= _runTimeData.RunSpeedMultiplier;

            if (_isWalking || _isRunning || _isSliding)
                _rb.velocity = new Vector2(_horizontal * speed, _rb.velocity.y);
        }

        private void HandleTurn()
        {
            _targetFacingRight = _horizontal > 0;
            if (_currentFacingRight == _targetFacingRight ||
                Mathf.Abs(_horizontal) <= _runTimeData.HorizontalInputThreshold)
            {
                _pendingTurnFacingRight = null;
                return;
            }

            if (_inGround && (_runTimeData.currentState == PlayerStats.Walk ||
                              _runTimeData.currentState == PlayerStats.Run))
            {
                _runTimeData.currentState = _runTimeData.currentState == PlayerStats.Walk
                    ? PlayerStats.WalkTurn
                    : PlayerStats.RunTurn;
                _pendingTurnFacingRight = _targetFacingRight;
            }
            else if (_runTimeData.currentState == PlayerStats.Jump || _runTimeData.currentState == PlayerStats.Fall)
            {
                _currentFacingRight = _targetFacingRight;
                transform.localRotation = Quaternion.Euler(0, (_currentFacingRight ? 0 : 180), 0);
                _pendingTurnFacingRight = null;
            }
        }

        private void TryAttack(PlayerStats state)
        {
            // 检测前方是否有敌人
            var hit = Physics2D.Raycast(transform.position,
                (_currentFacingRight ? Vector2.right : Vector2.left), 1f);
            if (hit.collider && hit.collider.CompareTag("Enemy"))
            {
                _runTimeData.AttackEnemy(hit.collider.gameObject, state);
            }
        }

        private void UpdateSlideCooldown()
        {
            if (!_isSlidingOnCooldown) return;
            _slideTimer -= Time.fixedDeltaTime;
            if (!(_slideTimer <= 0)) return;
            _isSlidingOnCooldown = false;
            _slideTimer = _runTimeData.SlideCool;
        }

        private void OnJumpPeakHandler()
        {
            if (_runTimeData.currentState == PlayerStats.Jump)
                _runTimeData.currentState = PlayerStats.Fall;
        }

        private void OnLandingHandler()
        {
            if (_runTimeData.currentState != PlayerStats.Fall) return;
            if (_isRunning)
                _runTimeData.currentState = PlayerStats.Run;
            else if (_isWalking)
                _runTimeData.currentState = PlayerStats.Walk;
            else
                _runTimeData.currentState = PlayerStats.Idle;
        }

        private void OnTurnEndHandler()
        {
            if (_runTimeData.currentState != PlayerStats.WalkTurn &&
                _runTimeData.currentState != PlayerStats.RunTurn)
                return;

            // 应用待处理的转身
            if (_pendingTurnFacingRight.HasValue)
            {
                _currentFacingRight = _pendingTurnFacingRight.Value;
                transform.localRotation = Quaternion.Euler(0, _currentFacingRight ? 0 : 180, 0);
                _pendingTurnFacingRight = null;
            }

            // 根据输入和地面状态恢复移动状态
            var wantsRun = Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(_horizontal) > 0.1f;
            var wantsWalk = Mathf.Abs(_horizontal) > _runTimeData.HorizontalInputThreshold;

            if (_inGround)
            {
                if (wantsRun)
                    _runTimeData.currentState = PlayerStats.Run;
                else if (wantsWalk)
                    _runTimeData.currentState = PlayerStats.Walk;
                else
                    _runTimeData.currentState = PlayerStats.Idle;
            }
            else
            {
                _runTimeData.currentState = PlayerStats.Fall;
            }
        }
    }
}