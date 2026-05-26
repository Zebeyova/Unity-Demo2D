using System;
using System.Collections.Generic;
using Script.Models;
using Script.RunTimeData;
using UnityEngine;

namespace Script.Views
{
    public class EnemyAnimView : MonoBehaviour
    {
        [SerializeField] private float crossFadeTime = 0.1f;

        private Animator _anim;
        private EnemyRunTimeData _runTimeData;
        private EnemyState _lastState;

        private static readonly Dictionary<EnemyState, int> AnimDictionary = new Dictionary<EnemyState, int>()
        {
            { EnemyState.Idle, Animator.StringToHash(nameof(EnemyState.Idle)) },
            { EnemyState.Walk, Animator.StringToHash(nameof(EnemyState.Walk)) },
            { EnemyState.Attack, Animator.StringToHash(nameof(EnemyState.Attack)) },
            { EnemyState.Hurt, Animator.StringToHash(nameof(EnemyState.Hurt)) },
            { EnemyState.Die, Animator.StringToHash(nameof(EnemyState.Die)) }
        };

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _runTimeData = GetComponent<EnemyRunTimeData>();
        }

        private void Start()
        {
            _runTimeData.OnHealthChanged += OnHealthChangedHandler;
        }

        private void OnDestroy()
        {
            if (!_runTimeData) return;
            _runTimeData.OnHealthChanged -= OnHealthChangedHandler;
        }

        private void Update()
        {
            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            if (_runTimeData.currentState == _lastState) return;
            _lastState = _runTimeData.currentState;
            if (AnimDictionary.TryGetValue(_lastState, out var hash)) PlayAnimation(hash);
        }

        private void PlayAnimation(int hash, float fade = -1)
        {
            if (!_anim) return;
            var fadeTime = fade < 0 ? crossFadeTime : fade;
            _anim.CrossFade(hash, fadeTime, 0);
        }

        private void OnHealthChangedHandler(float current, float max)
        {
            _runTimeData.currentState = current <= 0 ? EnemyState.Die : EnemyState.Hurt;
        }

        #region 动画事件调用

        public event Action OnAttackPlayer;
        public event Action OnAttackEnd;
        public event Action OnHurtEnd;
        public event Action OnDeathEnd;
        public void TriggerAttackPlayer() => OnAttackPlayer?.Invoke();
        public void TriggerAttackEnd() => OnAttackEnd?.Invoke();
        public void TriggerHurtEnd() => OnHurtEnd?.Invoke();
        public void TriggerDeathEnd() => OnDeathEnd?.Invoke();

        #endregion
    }
}