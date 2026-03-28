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
            var horizontal = Input.GetAxis("Horizontal");
            var isCurrentlyTurning = _animationController.InTurnState();

            _targetFacing = horizontal > 0;
            _isWalking = Mathf.Abs(horizontal) > moveThreshold;
            _isRunning = _isWalking && Input.GetKey(KeyCode.LeftShift) && !isCurrentlyTurning;
            _isSliding = _isRunning && Input.GetKey(KeyCode.Space) && !_isSlidingOnCoolDown && !isCurrentlyTurning;
            _isJumping = !_inJumping && Input.GetKeyDown(KeyCode.K);

            if (_isJumping && _inGround)
            {
                if (!_inJumping)
                {
                    _animationController.JumpAnimation(true);
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
                _isSlidingOnCoolDown = true;
                return;
            }

            if (_isWalking && !_isSliding && _currentFacing != _targetFacing && !isCurrentlyTurning)
            {
                _animationController.StartTurn(_isRunning, _isSliding, () =>
                {
                    _currentFacing = _targetFacing;
                    _spriteRenderer.flipX = !_currentFacing;
                });
                _animationController.UpdateState(_isWalking, _isRunning);
                return;
            }

            if (isCurrentlyTurning) return;
            _animationController.UpdateState(_isWalking, _isRunning);
        }

        private void MoveOperation()
        {
            if (_inJumping) return;
            if (_inGround && _isJumping)
            {
                _rb2D.velocity = new Vector2(_rb2D.velocity.x, 0);
                _rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }

            if (!_inGround || _isJumping) return;
            var currentSpeed = baseSpeed;
            if (_isRunning) currentSpeed *= runSpeedMultiplier;
            if (_isSliding) currentSpeed *= runSpeedMultiplier;

            var horizontal = Input.GetAxis("Horizontal");
            _rb2D.velocity = new Vector2(horizontal * currentSpeed, _rb2D.velocity.y);
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
        public float moveThreshold = 0.1f;
        public float runSpeedMultiplier = 2f;
        public float slideCool = 1f;
        public float jumpForce = 8f;

        #endregion

        #region 私有成员

        private bool _currentFacing;
        private bool _targetFacing;
        private bool _inGround;

        private bool _isRunning;
        private bool _isWalking;
        private bool _isJumping;
        private bool _inJumping;
        
        private bool _isSliding;
        private bool _isSlidingOnCoolDown;
        private float _slideTimer;

        #endregion
    }
}