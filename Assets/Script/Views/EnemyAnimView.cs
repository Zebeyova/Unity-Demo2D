using Script.Models;
using Script.RunTimeData;
using UnityEngine;

namespace Script.Views
{
    public class EnemyAnimView : MonoBehaviour
    {
        [SerializeField] private float crossFadeTime = 0.1f;

        private Animator _anim;
        private EnemyRunTimeData _data;
        private Coroutine _currentAnimCoroutine;

        // 动画哈希值
        private readonly int _idle = Animator.StringToHash("Idle");
        private readonly int _walk = Animator.StringToHash("Walk");
        private readonly int _attack = Animator.StringToHash("Attack");
        private readonly int _hurt = Animator.StringToHash("PlayerHurt");
        private readonly int _die = Animator.StringToHash("Die");

        // 状态跟踪
        private EnemyState _lastState;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _data = GetComponent<EnemyRunTimeData>();
        }

        private void Start()
        {
            _data.OnHealthChanged += OnHealthChangedHandler;
            _data.OnDeath += OnDeathHandler;
        }

        private void OnDestroy()
        {
            if (_data != null)
            {
                _data.OnHealthChanged -= OnHealthChangedHandler;
                _data.OnDeath -= OnDeathHandler;
            }
        }

        private void Update()
        {
            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            if (_data.currentState != _lastState)
            {
                _lastState = _data.currentState;

                switch (_data.currentState)
                {
                    case EnemyState.Idle:
                        PlayAnimation(_idle);
                        break;
                    case EnemyState.Walk:
                        PlayAnimation(_walk);
                        break;
                    case EnemyState.Attack:
                        PlayAnimation(_attack);
                        break;
                    case EnemyState.Hurt:
                        PlayAnimation(_hurt);
                        break;
                    case EnemyState.Die:
                        PlayAnimation(_die);
                        break;
                }
            }
        }

        private void OnHealthChangedHandler(float current, float max)
        {
            if (current <= 0)
            {
                _data.currentState = EnemyState.Die;
            }
            else
            {
                _data.currentState = EnemyState.Hurt;
            }
        }

        private void OnDeathHandler()
        {
            _data.currentState = EnemyState.Die;
        }

        private void PlayAnimation(int hash, float fade = -1)
        {
            if (_anim == null) return;
            float fadeTime = fade < 0 ? crossFadeTime : fade;
            _anim.CrossFade(hash, fadeTime, 0);
        }
    }
}