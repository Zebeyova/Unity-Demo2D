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

            _isTurning = false;
            _isWalking = false;
            _isRunning = false;
        }

        private void FixedUpdate()
        {
            PlayerMove();
        }

        private void PlayerMove()
        {
            var horizontal = Input.GetAxis("Horizontal");
            _isWalking = Mathf.Abs(horizontal) > _moveThreshold;

            var shouldRun = _isWalking && Input.GetKey(KeyCode.LeftShift) && !_isTurning;

            if (_isRunning != shouldRun) _isRunning = shouldRun;
            var wantToRight = horizontal > 0; //想要朝哪里转向
            if (_isWalking && !_isTurning)
            {
                if (_currentFacing != wantToRight)
                {
                    _targetFacing = wantToRight;
                    _isTurning = true;
                    _animationController.StartTurn(_isRunning, () =>
                    {
                        _currentFacing = _targetFacing;
                        _spriteRenderer.flipX = !_currentFacing;
                        _isTurning = false;
                    });
                    return;
                }
            }            
            var currentSpeed = speed;
            if (_isTurning) return;
            if (_isRunning) currentSpeed *= runSpeedMultiplier;
            
            _animationController.UpdateState(_isWalking, _isRunning);

            var targetPosition =
                _rigidbody2D.position + new Vector2(horizontal * currentSpeed * Time.fixedDeltaTime, 0);
            _rigidbody2D.MovePosition(targetPosition);
        }
    }
}