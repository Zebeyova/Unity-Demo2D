using System;
using System.Collections.Generic;
using Script.Interfaces;
using Script.RunTimeDatas;
using UnityEngine;

namespace Script.Views
{
    public class EnemyAnimView : MonoBehaviour
    {
        private Animator _anim;
        private EnemyRunTimeData _runTimeData;
        private ICharacterValue.Stats _lastStats;
        private const float CrossFadeTime = 0.1f;

        private static readonly Dictionary<ICharacterValue.Stats, int> AnimDictionary =
            new Dictionary<ICharacterValue.Stats, int>()
            {
                { ICharacterValue.Stats.Idle, Animator.StringToHash(nameof(ICharacterValue.Stats.Idle)) },
                { ICharacterValue.Stats.Walk, Animator.StringToHash(nameof(ICharacterValue.Stats.Walk)) },
                { ICharacterValue.Stats.Attack, Animator.StringToHash(nameof(ICharacterValue.Stats.Attack)) },
                { ICharacterValue.Stats.Hurt, Animator.StringToHash(nameof(ICharacterValue.Stats.Hurt)) },
                { ICharacterValue.Stats.Death, Animator.StringToHash(nameof(ICharacterValue.Stats.Death)) }
            };

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _runTimeData = GetComponent<EnemyRunTimeData>();
        }

        private void Update()
        {
            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            if (_runTimeData.currentStats == _lastStats) return;
            _lastStats = _runTimeData.currentStats;
            if (AnimDictionary.TryGetValue(_lastStats, out var hash)) PlayAnimation(hash);
        }

        private void PlayAnimation(int hash, float fade = -1)
        {
            if (!_anim) return;
            var fadeTime = fade < 0 ? CrossFadeTime : fade;
            _anim.CrossFade(hash, fadeTime, 0);
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