using System;
using System.Collections.Generic;
using Script.Interfaces;
using Script.RunTimeDatas;
using UnityEngine;

namespace Script.Views
{
    public class PlayerAnimView : MonoBehaviour
    {
        [SerializeField] private float crossFadeTime = 0.1f;

        private Animator _anim;
        private PlayerRunTimeData _runTimeData;
        private ICharacterValue.Stats _lastState;

        private static readonly Dictionary<ICharacterValue.Stats, int> AnimDictionary = new Dictionary<ICharacterValue.Stats, int>
        {
            { ICharacterValue.Stats.Idle, Animator.StringToHash(nameof(ICharacterValue.Stats.Idle)) },
            { ICharacterValue.Stats.Walk, Animator.StringToHash(nameof(ICharacterValue.Stats.Walk)) },
            { ICharacterValue.Stats.Run, Animator.StringToHash(nameof(ICharacterValue.Stats.Run)) },
            { ICharacterValue.Stats.WalkTurn, Animator.StringToHash(nameof(ICharacterValue.Stats.WalkTurn)) },
            { ICharacterValue.Stats.RunTurn, Animator.StringToHash(nameof(ICharacterValue.Stats.RunTurn)) },
            { ICharacterValue.Stats.Slide, Animator.StringToHash(nameof(ICharacterValue.Stats.Slide)) },
            { ICharacterValue.Stats.Jump, Animator.StringToHash(nameof(ICharacterValue.Stats.Jump)) },
            { ICharacterValue.Stats.Fall, Animator.StringToHash(nameof(ICharacterValue.Stats.Fall)) },
            { ICharacterValue.Stats.FallLoop, Animator.StringToHash(nameof(ICharacterValue.Stats.FallLoop)) },
            { ICharacterValue.Stats.Attack, Animator.StringToHash(nameof(ICharacterValue.Stats.Attack)) },
            { ICharacterValue.Stats.Attack2, Animator.StringToHash(nameof(ICharacterValue.Stats.Attack2)) },
            { ICharacterValue.Stats.Skill, Animator.StringToHash(nameof(ICharacterValue.Stats.Skill)) },
            { ICharacterValue.Stats.Hurt, Animator.StringToHash(nameof(ICharacterValue.Stats.Hurt)) },
            { ICharacterValue.Stats.Death, Animator.StringToHash(nameof(ICharacterValue.Stats.Death)) }
        };

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _runTimeData = GetComponent<PlayerRunTimeData>();
        }

        private void Start()
        {
            _runTimeData.OnPlayerHurt += OnPlayerPlayerHealthChangedHandler;
        }

        private void OnDestroy()
        {
            if (!_runTimeData) return;
            _runTimeData.OnPlayerHurt -= OnPlayerPlayerHealthChangedHandler;
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

        private void OnPlayerPlayerHealthChangedHandler(float current, float max)
        {
            _runTimeData.currentState = current <= 0 ? ICharacterValue.Stats.Death : ICharacterValue.Stats.Hurt;
        }

        #region 动画事件回调

        public event Action OnJumpPeak;
        public event Action OnLanding;
        public event Action OnTurnEnd;
        public event Action OnSlideEnd;
        public event Action OnAttackEnd;
        public event Action OnHurtEnd;
        public void TriggerJumpPeak() => OnJumpPeak?.Invoke();
        public void TriggerLanding() => OnLanding?.Invoke();
        public void TriggerTurnEnd() => OnTurnEnd?.Invoke();
        public void TriggerSlideEnd() => OnSlideEnd?.Invoke();
        public void TriggerAttackEnd() => OnAttackEnd?.Invoke();
        public void TriggerHurtEnd() => OnHurtEnd?.Invoke();

        #endregion
    }
}