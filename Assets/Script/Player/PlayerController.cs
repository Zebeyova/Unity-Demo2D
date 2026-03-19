using UnityEngine;

namespace Script.Player
{
    public class PlayerController : MonoBehaviour
    {
        public float speed = 2f;
        public float runSpeedMultiplier = 2f; // 跑步速度倍数
        private readonly float _moveThreshold = 0.1f; //可以移动的最小阈值

        private PlayerAnimationController _animationController;
        private bool _currentFacing; //true代表面向右侧(当前朝向)
        private bool _isRunning;
        private bool _isTurning; //false代表不在转向中(是否在转向动画中)
        private bool _isWalking;
        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;
        private bool _targetFacing; //true代表右侧(目标朝向)

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _currentFacing = !_spriteRenderer.flipX;
            _animationController = GetComponent<PlayerAnimationController>();
            _animationController.SetBool(_animationController.isTurnComplete, true);
            _animationController.SetBool(_animationController.idleWalk, false);
            _animationController.OnTurnComplete += HandleTurnComplete;

            _isTurning = false;
            _isWalking = false;
            _isRunning = false;
        }

        private void FixedUpdate()
        {
            PlayerMove();
        }

        private void OnDestroy()
        {
            if (_animationController != null)
                _animationController.OnTurnComplete -= HandleTurnComplete;
        }

        private void PlayerMove()
        {
            var horizontal = Input.GetAxis("Horizontal");
            _isWalking = Mathf.Abs(horizontal) > _moveThreshold;

            var shouldRun = _isWalking && Input.GetKey(KeyCode.LeftShift) && !_isTurning;

            if (_isRunning != shouldRun)
            {
                _isRunning = shouldRun;

                if (shouldRun)
                {
                    _isRunning = true;
                    _animationController.SetBool(_animationController.walkRun, true);
                    _animationController.SetBool(_animationController.idleWalk, false);
                }
                else
                {
                    _isRunning = false;
                    _animationController.SetBool(_animationController.walkRun, false);
                    _animationController.SetBool(_animationController.idleWalk, !_isWalking);
                }
            }

            if (!_isRunning) _animationController.SetBool(_animationController.idleWalk, _isWalking);

            var wantToRight = horizontal > 0; //想要朝哪里转向
            if (_isWalking && !_isTurning)
                if (wantToRight != _currentFacing)
                {
                    _targetFacing = wantToRight;
                    _isTurning = true;
                    _animationController.SetBool(_animationController.isTurnComplete, false);
                    _animationController.SetTrigger(_animationController.walkTurn);
                    _animationController.SetBool(_animationController.idleWalk, true);
                    return;
                }

            if (_isTurning) return;

            var currentSpeed = speed;
            if (_isRunning) currentSpeed *= runSpeedMultiplier;

            var targetPosition =
                _rigidbody2D.position + new Vector2(horizontal * currentSpeed * Time.fixedDeltaTime, 0);
            _rigidbody2D.MovePosition(targetPosition);
        }

        #region 动画事件

        private void HandleTurnComplete()
        {
            _currentFacing = _targetFacing;
            _spriteRenderer.flipX = !_currentFacing;
            _animationController.SetBool(_animationController.isTurnComplete, true);
            _isTurning = false;
        }

        #endregion
    }
}