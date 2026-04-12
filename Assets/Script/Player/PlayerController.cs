using UnityEngine;

namespace Script.Player
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Run,
        Slide,
        Jump,
        Attack
    }

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
            InputCheck();
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
            if (!_cr2D) _cr2D = GetComponent<Collider2D>();
            if (!_rb2D) _rb2D = GetComponent<Rigidbody2D>();
            if (!_animationController) _animationController = GetComponent<PlayerAnimationController>();
            if (!_properties) _properties = FindObjectOfType<PlayerProperties>();
        }

        private void ChangeState()
        {
            _horizontal = Input.GetAxis("Horizontal");

            if (_isAttacking)
            {
                if (!Input.GetKeyDown(KeyCode.J)) return;
                _animationController.ComboRequest(2);
                return;
            }

            if (_currentState == PlayerState.Run && Input.GetKeyDown(KeyCode.Space) && _inGround &&
                !_isSlidingOnCoolDown)
            {
                _currentState = PlayerState.Slide;
                return;
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                _currentState = PlayerState.Jump;
                return;
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                _animationController.ComboRequest(3);
                _currentState = PlayerState.Attack;
                return;
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                _animationController.ComboRequest(1);
                _currentState = PlayerState.Attack;
                return;
            }

            if (Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(_horizontal) > 0)
            {
                _currentState = PlayerState.Run;
                return;
            }

            if (Mathf.Abs(_horizontal) > 0)
            {
                _currentState = PlayerState.Walk;
                return;
            }

            _currentState = PlayerState.Idle;
        }

        private void InputCheck()
        {
            _isWalking = _isRunning = _isSliding = _isJumping = _isAttacking = false;
            switch (_currentState)
            {
                case PlayerState.Idle:
                    break;
                case PlayerState.Walk:
                    _isWalking = true;
                    break;
                case PlayerState.Run:
                    _isRunning = Mathf.Abs(_horizontal) > 0;
                    break;
                case PlayerState.Slide:
                    _isSliding = true;
                    break;
                case PlayerState.Jump:
                    _isJumping = true;
                    break;
                case PlayerState.Attack:
                    _isAttacking = true;
                    break;
            }
        }

        private void PlayerControl()
        {
            MoveOperation();
            if (_isAttacking) return;

            _inTurning = _animationController.InTurnState();

            if (_isJumping && _inGround)
            {
                _rb2D.velocity = new Vector2(_rb2D.velocity.x, _properties.jumpForce);
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
                             Mathf.Abs(_horizontal) > _properties.horizontalInputThreshold;
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

            var currentSpeed = _properties.baseSpeed;
            if (_isSliding) currentSpeed *= _properties.runSpeedMultiplier * 1.35f;
            else if (_isRunning) currentSpeed *= _properties.runSpeedMultiplier;

            if (_isWalking || _isRunning)
                _rb2D.velocity = new Vector2(_horizontal * currentSpeed, _rb2D.velocity.y);
        }

        private void SlideTimer()
        {
            if (_isSlidingOnCoolDown) _slideTimer -= Time.fixedDeltaTime;
            if (_slideTimer > 0) return;
            _isSlidingOnCoolDown = false;
            _slideTimer = _properties.slideCool;
        }

        public void OnAttackFinished()
        {
            comboCount = 0;
            _currentState = PlayerState.Idle;
        }

        public void DestroyPlayer()
        {
            Destroy(gameObject);
        }

        #region 成员

        public int comboCount;
        public LayerMask groundLayerMask;
        private PlayerAnimationController _animationController;
        private PlayerProperties _properties;
        private Collider2D _cr2D;
        private PlayerState _currentState = PlayerState.Idle;
        private Rigidbody2D _rb2D;

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

        #endregion
    }
}