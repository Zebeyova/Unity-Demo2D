using UnityEngine;

namespace Script.Player
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerAnimationController _animationController;
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rigidbody2D;

        #region 属性配置

        public float speed = 2f;
        private readonly float _moveThreshold = 0.1f; //可以移动的最小阈值
        public float runSpeedMultiplier = 2f; // 跑步速度倍数
        public float slideCool = 1f; //滑铲冷却

        #endregion

        #region 私有成员

        private bool _currentFacing; //true代表面向右侧(当前朝向)
        private bool _targetFacing; //true代表右侧(目标朝向)
        private bool _isRunning;
        private bool _isTurning;
        private bool _isWalking;
        private bool _isSliding;
        private bool _isSlidingOnCoolDown; //滑铲是否在冷却
        private float _slideTimer; //滑铲计时器

        #endregion

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _currentFacing = !_spriteRenderer.flipX;
            _animationController = GetComponent<PlayerAnimationController>();

            _isTurning = false;
            _isWalking = false;
            _isRunning = false;
            _isSliding = false;
            _isSlidingOnCoolDown = false;
            _slideTimer = slideCool;
        }

        private void FixedUpdate()
        {
            PlayerMove();
            SlideTimer();
        }

        private void PlayerMove()
        {
            var horizontal = Input.GetAxis("Horizontal");
            _isWalking = Mathf.Abs(horizontal) > _moveThreshold;
            var canSlide = _isRunning && Input.GetKey(KeyCode.Space) && !_isSlidingOnCoolDown;
            _isSliding = canSlide && !_isTurning;

            var shouldRun = _isWalking && Input.GetKey(KeyCode.LeftShift) && !_isTurning;
            if (_isRunning != shouldRun) _isRunning = shouldRun;
            if (_isWalking && !_isSliding)
            {
                var wantToRight = horizontal > 0; //想要朝哪里转向
                if (_currentFacing != wantToRight) //转身相关
                {
                    _targetFacing = wantToRight;
                    _isTurning = true;
                    _animationController.StartTurn(_isRunning, _isSliding, () =>
                    {
                        _currentFacing = _targetFacing;
                        _spriteRenderer.flipX = !_currentFacing;
                        _isTurning = false;
                    });
                    return;
                }
            }

            if (_isSliding)
            {
                _animationController.RunAnimation(_isSliding);
                _isSlidingOnCoolDown = true;
            }

            var currentSpeed = speed;
            if (_isTurning) return;
            if (_isRunning) currentSpeed *= runSpeedMultiplier;
            if (_isSliding) currentSpeed *= runSpeedMultiplier;

            _animationController.UpdateState(_isWalking, _isRunning);

            var targetPosition =
                _rigidbody2D.position + new Vector2(horizontal * currentSpeed * Time.fixedDeltaTime, 0);
            _rigidbody2D.MovePosition(targetPosition);
        }

        private void SlideTimer() //滑铲计时器
        {
            if (_isSlidingOnCoolDown) _slideTimer -= Time.fixedDeltaTime;
            if (_slideTimer > 0) return;
            _isSlidingOnCoolDown = false;
            _slideTimer = slideCool;
        }
    }
}