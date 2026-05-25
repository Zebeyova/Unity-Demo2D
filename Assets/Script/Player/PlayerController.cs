using Resources;
using UnityEngine;

namespace Script.Player
{
    public class PlayerController : MonoBehaviour
    {
        private void Awake()
        {
            CheckComponent();
            _currentFacing = transform.localRotation.y == 0;
        }

        private void Update()
        {
            _inGround = _cr2D.IsTouchingLayers(groundLayerMask);
            ChangeState();
            PlayerControl();
        }

        private void FixedUpdate()
        {
            MoveOperation();
            SlideTimer();
        }

        private void CheckComponent()
        {
            _cr2D = GetComponent<Collider2D>();
            _rb2D = GetComponent<Rigidbody2D>();
            _runData = GetComponent<PlayerRunningData>();
            _animationController = GetComponent<PlayerAnimationController>();
        }

        private void ChangeState()
        {
            _horizontal = Input.GetAxis("Horizontal");
            _isWalking = _isRunning = _isSliding = _isJumping = _isAttacking = false;

            if ((_runData.currentState == PlayerStats.Run) && Input.GetKeyDown(KeyCode.Space) && _inGround &&
                !_isSlidingOnCoolDown) //滑铲
            {
                _runData.currentState = PlayerStats.Slide;
                _isSliding = true;
                return;
            }

            if (Input.GetKeyDown(KeyCode.K)) //跳跃
            {
                _runData.currentState = PlayerStats.Jump;
                _isJumping = true;
                return;
            }

            if (Input.GetKeyDown(KeyCode.L)) //攻技能
            {
                _runData.currentState = PlayerStats.Skills;
                _animationController.AttackAnimation(3);
                _isAttacking = true;
                return;
            }

            if (Input.GetKeyDown(KeyCode.J)) //攻击
            {
                if (_isAttacking && _runData.comboCount > 5) //二段连击
                {
                    _runData.currentState = PlayerStats.Attack2;
                    _animationController.AttackAnimation(2);
                    _runData.comboCount = 0;
                    return;
                }

                _runData.currentState = PlayerStats.Attack1;
                _animationController.AttackAnimation(1);
                _isAttacking = true;
                _runData.comboCount++;
                return;
            }

            if (Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(_horizontal) > 0) //按住跑步
            {
                _runData.currentState = PlayerStats.Run;
                _isRunning = Mathf.Abs(_horizontal) > 0;
                return;
            }

            if (Mathf.Abs(_horizontal) > 0) //前进
            {
                _runData.currentState = PlayerStats.Walk;
                _isWalking = true;
                return;
            }

            _runData.currentState = PlayerStats.Idle; //待机
        }

        private void PlayerControl()
        {
            MoveOperation();
            if (_isAttacking) return;

            _inTurning = _animationController.TurnState();

            if (_isJumping && _inGround)
            {
                _rb2D.velocity = new Vector2(_rb2D.velocity.x, _runData.JumpForce);
                _animationController.JumpAnimation(true);
                _animationController.UpdateState(_isWalking, _isRunning);
                _isJumping = false;
                return;
            }

            JumpTurn();

            if (_isSliding)
            {
                MoveOperation();
                _animationController.RunAnimation(_isSliding);
                _isSlidingOnCoolDown = true;
                _animationController.UpdateState(_isWalking, _isRunning);
                return;
            }

            Turn();
            if (_inTurning) return;
            _animationController.UpdateState(_isWalking, _isRunning);
        }

        private void Turn()
        {
            _horizontal = Input.GetAxis("Horizontal");
            _targetFacing = _horizontal > 0;
            var shouldTurn = !_inTurning && _currentFacing != _targetFacing &&
                             (_isWalking || _isRunning) && !_isSliding &&
                             Mathf.Abs(_horizontal) > _runData.HorizontalInputThreshold;
            if (!shouldTurn) return;

            _animationController.StartTurn(_isRunning, () =>
            {
                _currentFacing = _targetFacing;
                transform.localRotation = Quaternion.Euler(0, _currentFacing ? 0 : 180, 0);
            });
            _animationController.UpdateState(_isWalking, _isRunning);
        }

        private void JumpTurn()
        {
            if (_inGround) return;
            _horizontal = Input.GetAxis("Horizontal");
            _targetFacing = _horizontal > 0;
            if (_currentFacing == _targetFacing || _horizontal == 0) return;
            _currentFacing = _targetFacing;
            transform.localRotation = Quaternion.Euler(0, _currentFacing ? 0 : 180, 0);
        }

        private void MoveOperation()
        {
            if (_isAttacking)
            {
                _rb2D.velocity = Vector2.zero;
                return;
            }

            var currentSpeed = _runData.BaseSpeed;
            if (_isSliding) currentSpeed *= _runData.RunSpeedMultiplier * 1.35f;
            else if (_isRunning) currentSpeed *= _runData.RunSpeedMultiplier;

            if (_isWalking || _isRunning)
                _rb2D.velocity = new Vector2(_horizontal * currentSpeed, _rb2D.velocity.y);
        }

        private void SlideTimer()
        {
            if (_isSlidingOnCoolDown) _slideTimer -= Time.fixedDeltaTime;
            if (_slideTimer > 0) return;
            _isSlidingOnCoolDown = false;
            _slideTimer = _runData.SlideCool;
        }

        public void OnAttackFinished()
        {
            _runData.comboCount = 0;
            _runData.currentState = PlayerStats.Idle;
        }

        public void DestroyPlayer() => Destroy(gameObject);

        #region 成员

        public LayerMask groundLayerMask;
        private PlayerRunningData _runData;
        private PlayerAnimationController _animationController;
        private Collider2D _cr2D;
        private Rigidbody2D _rb2D;

        #endregion

        #region 属性

        private float _horizontal;
        private bool _inTurning;
        private bool _currentFacing;
        private bool _targetFacing;
        private bool _inGround;
        private bool _isRunning;
        private bool _isWalking;
        private bool _isJumping;
        private bool _isAttacking;
        private bool _isSliding;
        private bool _isSlidingOnCoolDown;
        private float _slideTimer;

        public float GetSlideTimer()
        {
            return _slideTimer;
        }

        #endregion
    }
}