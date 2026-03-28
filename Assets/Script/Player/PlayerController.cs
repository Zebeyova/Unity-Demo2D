using UnityEngine;

namespace Script.Player
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerAnimationController _animationController;
        private Collider2D _cr2D;
        public LayerMask groundLayerMask;
        private Rigidbody2D _rb2D;
        private SpriteRenderer _spriteRenderer;

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
            InputCheck();
            SlideTimer();
            _inGround = _cr2D.IsTouchingLayers(groundLayerMask);
        }

        private void FixedUpdate()
        {
            PlayerMove();
            MoveOperation();
        }

        private void PlayerMove()
        {
            if (_isJumping && _inGround)
            {
                if (!_inJumping)
                {
                    _animationController.JumpAnimation(true);
                    _animationController.UpdateState(_isWalking, _isRunning);
                    _inJumping = true;
                }

                if (_inJumping)
                {
                    _inJumping = false;
                }
            }

            if (_isSliding)
            {
                _animationController.RunAnimation(_isSliding);
                _animationController.UpdateState(_isWalking, _isRunning);
                _isSlidingOnCoolDown = true;
                return;
            }

            if (_isWalking && !_isSliding && _currentFacing != _targetFacing && !_inTurning)
            {
                _animationController.StartTurn(_isRunning, _isSliding, () =>
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

        private void InputCheck()
        {
            _horizontal = Input.GetAxis("Horizontal");
            _inTurning = _animationController.InTurnState();
            _targetFacing = _horizontal > 0;
            _isWalking = Mathf.Abs(_horizontal) > 0;
            _isRunning = _isWalking && Input.GetKey(KeyCode.LeftShift);
            _isSliding = _isRunning && Input.GetKey(KeyCode.Space) && !_isSlidingOnCoolDown;
            _isJumping = !_inJumping && Input.GetKeyDown(KeyCode.K);
        }

        private void MoveOperation()
        {
            if (_inJumping) return;
            if (_inGround && _isJumping) _rb2D.velocity = new Vector2(_rb2D.velocity.x, jumpForce);
            if (_isJumping) return;
            var currentSpeed = baseSpeed;
            if (_isRunning) currentSpeed *= runSpeedMultiplier;
            if (_isSliding) currentSpeed *= runSpeedMultiplier;
            _rb2D.velocity = new Vector2(_horizontal * currentSpeed, _rb2D.velocity.y);
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