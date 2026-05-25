using System;
using Script.Models;
using Script.RunTimeData;
using UnityEngine;

namespace Script.Views
{
    public class PlayerAnimView : MonoBehaviour
    {
        [SerializeField] private float crossFadeTime = 0.1f;

        private Animator _anim;
        private PlayerRunTimeData _runTimeData;
        private PlayerStats _lastState;
        public event Action OnJumpPeak;
        public event Action OnLanding;

        // 动画状态哈希值
        private readonly int _idle = Animator.StringToHash(nameof(PlayerStats.Idle));
        private readonly int _walk = Animator.StringToHash(nameof(PlayerStats.Walk));
        private readonly int _run = Animator.StringToHash(nameof(PlayerStats.Run));
        private readonly int _walkTurn = Animator.StringToHash(nameof(PlayerStats.WalkTurn));
        private readonly int _runTurn = Animator.StringToHash(nameof(PlayerStats.RunTurn));
        private readonly int _slide = Animator.StringToHash(nameof(PlayerStats.Slide));
        private readonly int _jump = Animator.StringToHash(nameof(PlayerStats.Jump));
        private readonly int _fall = Animator.StringToHash(nameof(PlayerStats.Fall));
        private readonly int _attack1 = Animator.StringToHash(nameof(PlayerStats.Attack1));
        private readonly int _attack2 = Animator.StringToHash(nameof(PlayerStats.Attack2));
        private readonly int _skills = Animator.StringToHash(nameof(PlayerStats.Skills));
        private readonly int _hurt = Animator.StringToHash(nameof(PlayerStats.Hurt));
        private readonly int _death = Animator.StringToHash(nameof(PlayerStats.Death));

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _runTimeData = GetComponent<PlayerRunTimeData>();
        }

        private void Start()
        {
            _runTimeData.OnHurt += OnHealthChangedHandler;
            _runTimeData.OnDeath += OnDeathHandler;
        }

        private void OnDestroy()
        {
            if (!_runTimeData) return;
            _runTimeData.OnHurt -= OnHealthChangedHandler;
            _runTimeData.OnDeath -= OnDeathHandler;
        }

        private void Update()
        {
            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            if (_runTimeData.currentState == _lastState) return;
            _lastState = _runTimeData.currentState;
            print(_runTimeData.currentState);
            switch (_runTimeData.currentState)
            {
                case PlayerStats.Idle:
                    PlayAnimation(_idle);
                    break;
                case PlayerStats.Walk:
                    PlayAnimation(_walk);
                    break;
                case PlayerStats.Run:
                    PlayAnimation(_run);
                    break;
                case PlayerStats.WalkTurn:
                    PlayAnimation(_walkTurn);
                    break;
                case PlayerStats.RunTurn:
                    PlayAnimation(_runTurn);
                    break;
                case PlayerStats.Slide:
                    PlayAnimation(_slide);
                    break;
                case PlayerStats.Jump:
                    PlayAnimation(_jump);
                    break;
                case PlayerStats.Fall:
                    PlayAnimation(_fall);
                    break;
                case PlayerStats.Attack1:
                    PlayAnimation(_attack1);
                    break;
                case PlayerStats.Attack2:
                    PlayAnimation(_attack2);
                    break;
                case PlayerStats.Skills:
                    PlayAnimation(_skills);
                    break;
                case PlayerStats.Hurt:
                    PlayAnimation(_hurt);
                    break;
                case PlayerStats.Death:
                    PlayAnimation(_death);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnHealthChangedHandler(float current, float max)
        {
            _runTimeData.currentState = current <= 0 ? PlayerStats.Death : PlayerStats.Hurt;
        }

        private void OnDeathHandler()
        {
            _runTimeData.currentState = PlayerStats.Death;
        }

        private void PlayAnimation(int hash, float fade = -1)
        {
            if (!_anim) return;
            var fadeTime = fade < 0 ? crossFadeTime : fade;
            _anim.CrossFade(hash, fadeTime, 0);
        }

        public void TriggerJumpPeak()
        {
            OnJumpPeak?.Invoke();
        }

        public void TriggerLanding()
        {
            OnLanding?.Invoke();
        }
    }
}