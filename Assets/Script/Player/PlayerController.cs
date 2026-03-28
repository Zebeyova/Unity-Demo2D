using UnityEngine;

namespace Script.Player
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Run,
        Slide,
        Jump
    }

    public class PlayerController : MonoBehaviour
    {
        private PlayerAnimationController _animationController;
        private Collider2D _cr2D;
        public LayerMask groundLayerMask;
        private Rigidbody2D _rb2D;
        private SpriteRenderer _spriteRenderer;
        private PlayerState _currentState = PlayerState.Idle;

        private void Awake()
        {
            _rb2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _currentFacing = !_spriteRenderer.flipX;
            _animationController = GetComponent<PlayerAnimationController>();
            _cr2D = GetComponent<Collider2D>();
        }

        private void Update()
        {
            ChangeState();
            PlayerControl();
            InputCheck();
            _inGround = _cr2D.IsTouchingLayers(groundLayerMask);
        }

        private void FixedUpdate()
        {
            MoveOperation();
            SlideTimer();
        }

        private void ChangeState()
        {
            _horizontal = Input.GetAxis("Horizontal");

            if (Input.GetKeyDown(KeyCode.K))
            {
                _currentState = PlayerState.Jump;
                return;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                _currentState = PlayerState.Run;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _currentState = PlayerState.Slide;
                }

                return;
            }

            if (Mathf.Abs(_horizontal) > 0)
            {
                _currentState = PlayerState.Walk;
            }
            else if (Mathf.Abs(_horizontal) == 0)
            {
                _currentState = PlayerState.Idle;
            }
        }

        private void InputCheck()
        {
            switch (_currentState)
            {
                case PlayerState.Idle:
                    _isWalking = _isRunning = _isSliding = _isJumping = false;
                    break;
                case PlayerState.Walk:
                    _isRunning = _isSliding = _isJumping = false;
                    _isWalking = true;
                    break;
                case PlayerState.Run:
                    _isWalking = _isSliding = _isJumping = false;
                    _isRunning = Mathf.Abs(_horizontal) > 0;
                    break;
                case PlayerState.Slide:
                    _isWalking = _isJumping = false;
                    _isRunning = true;
                    _isSliding = _isRunning && !_isSlidingOnCoolDown;
                    break;
                case PlayerState.Jump:
                    _isWalking = _isRunning = _isSliding = false;
                    _isJumping = true;
                    print(_inJumping.ToString());
                    break;
            }
        }

        private void PlayerControl()
        {
            if (_isJumping && _inGround) //跳跃
            {
                if (!_inJumping)
                {
                    _animationController.JumpAnimation(true);
                    _inJumping = true;
                    MoveOperation();
                    _animationController.UpdateState(_isWalking, _isRunning);
                    return;
                }

                if (_inJumping)
                {
                    _inJumping = false;
                    return;
                }
            }

            if (_isSliding) //滑铲
            {
                _animationController.RunAnimation(_isSliding);
                _isSlidingOnCoolDown = true;
                _animationController.UpdateState(_isWalking, _isRunning);
                return;
            }

            _targetFacing = _horizontal > 0;
            _inTurning = _animationController.InTurnState();

            var shouldTurn = (!_inTurning && _currentFacing != _targetFacing &&
                              Mathf.Abs(_horizontal) > horizontalInputThreshold &&
                              (_isWalking || _isRunning) && !_isSliding);

            if (shouldTurn) //转向
            {
                _animationController.StartTurn(_isRunning, () =>
                {
                    _currentFacing = _targetFacing;
                    _spriteRenderer.flipX = !_currentFacing;
                });
                _animationController.UpdateState(_isWalking, _isRunning);
                return;
            }

            if (_inTurning) return;
            _animationController.UpdateState(_isWalking, _isRunning);
        }

        private void MoveOperation()
        {
            if (_inGround && _isJumping) _rb2D.velocity = new Vector2(_rb2D.velocity.x, jumpForce);
            var currentSpeed = baseSpeed;
            if (_isRunning) currentSpeed *= runSpeedMultiplier;
            if (_isSliding) currentSpeed *= runSpeedMultiplier;
            if (_isWalking || _isRunning) _rb2D.velocity = new Vector2(_horizontal * currentSpeed, _rb2D.velocity.y);
        }

        private void SlideTimer()
        {
            if (_isSlidingOnCoolDown) _slideTimer -= Time.fixedDeltaTime;
            if (_slideTimer > 0) return;
            _isSlidingOnCoolDown = false;
            _slideTimer = slideCool;
        }

        #region 属性配置

        public float baseSpeed = 2f;
        public float runSpeedMultiplier = 2f;
        public float slideCool = 1f;
        public float jumpForce = 8f;
        public float horizontalInputThreshold = 0.02f; //水平输入阈值

        #endregion

        #region 私有成员

        private float _horizontal;
        private bool _inTurning; //在转身中
        private bool _currentFacing;
        private bool _targetFacing;
        private bool _inGround;

        private bool _isRunning;
        private bool _isWalking;
        private bool _isJumping; //准备跳跃
        private bool _inJumping;

        private bool _isSliding;
        private bool _isSlidingOnCoolDown;
        private float _slideTimer;

        #endregion
    }
}