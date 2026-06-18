using System;
using System.Collections;
using System.Collections.Generic;
using Script.Interfaces;
using Script.RunTimeDatas;
using UnityEngine;

namespace Script.Views
{
    public class PlayerAnimView : MonoBehaviour
    {
        [SerializeField] private float crossFadeTime = 0.1f; //动画过渡时间

        private Animator _anim;
        private PlayerRunTimeData _runTimeData;
        private IDamageable.Stats _lastState;
        private Coroutine _restoreStateCoroutine;

        private static readonly Dictionary<IDamageable.Stats, int> AnimDictionary =
            new Dictionary<IDamageable.Stats, int>
            {
                { IDamageable.Stats.Idle, Animator.StringToHash(nameof(IDamageable.Stats.Idle)) },
                { IDamageable.Stats.Walk, Animator.StringToHash(nameof(IDamageable.Stats.Walk)) },
                { IDamageable.Stats.Run, Animator.StringToHash(nameof(IDamageable.Stats.Run)) },
                { IDamageable.Stats.WalkTurn, Animator.StringToHash(nameof(IDamageable.Stats.WalkTurn)) },
                { IDamageable.Stats.RunTurn, Animator.StringToHash(nameof(IDamageable.Stats.RunTurn)) },
                { IDamageable.Stats.Slide, Animator.StringToHash(nameof(IDamageable.Stats.Slide)) },
                { IDamageable.Stats.Jump, Animator.StringToHash(nameof(IDamageable.Stats.Jump)) },
                { IDamageable.Stats.Fall, Animator.StringToHash(nameof(IDamageable.Stats.Fall)) },
                { IDamageable.Stats.FallLoop, Animator.StringToHash(nameof(IDamageable.Stats.FallLoop)) },
                { IDamageable.Stats.Attack, Animator.StringToHash(nameof(IDamageable.Stats.Attack)) },
                { IDamageable.Stats.Attack2, Animator.StringToHash(nameof(IDamageable.Stats.Attack2)) },
                { IDamageable.Stats.Skill, Animator.StringToHash(nameof(IDamageable.Stats.Skill)) },
                { IDamageable.Stats.Hurt, Animator.StringToHash(nameof(IDamageable.Stats.Hurt)) },
                { IDamageable.Stats.Death, Animator.StringToHash(nameof(IDamageable.Stats.Death)) }
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
            if (_runTimeData.currentState == IDamageable.Stats.Attack ||
                _runTimeData.currentState == IDamageable.Stats.Attack2 ||
                _runTimeData.currentState == IDamageable.Stats.Skill ||
                _runTimeData.currentState == IDamageable.Stats.Hurt)
            {
                if (_restoreStateCoroutine != null) StopCoroutine(_restoreStateCoroutine);
                _restoreStateCoroutine = StartCoroutine(RestoreState());
            }

            var fadeTime = fade < 0 ? crossFadeTime : fade;
            _anim.CrossFade(hash, fadeTime, 0);
        }

        private IEnumerator RestoreState()
        {
            yield return null;
            var animLength = _anim.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animLength - crossFadeTime);
            if (_runTimeData.currentState == IDamageable.Stats.Hurt) OnHurtEnd?.Invoke();
            else OnAttackEnd?.Invoke();
            _restoreStateCoroutine = null;
        }

        private void OnPlayerPlayerHealthChangedHandler(float current, float max)
        {
            _runTimeData.currentState = current <= 0 ? IDamageable.Stats.Death : IDamageable.Stats.Hurt;
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