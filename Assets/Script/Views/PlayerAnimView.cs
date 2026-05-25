using System;
using System.Collections.Generic;
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
        public event Action OnTurnEnd;
        public event Action OnSlideEnd;
        public event Action OnAttackEnd;

        private static readonly Dictionary<PlayerStats, int> AnimDictionary = new Dictionary<PlayerStats, int>
        {
            { PlayerStats.Idle, Animator.StringToHash(nameof(PlayerStats.Idle)) },
            { PlayerStats.Walk, Animator.StringToHash(nameof(PlayerStats.Walk)) },
            { PlayerStats.Run, Animator.StringToHash(nameof(PlayerStats.Run)) },
            { PlayerStats.WalkTurn, Animator.StringToHash(nameof(PlayerStats.WalkTurn)) },
            { PlayerStats.RunTurn, Animator.StringToHash(nameof(PlayerStats.RunTurn)) },
            { PlayerStats.Slide, Animator.StringToHash(nameof(PlayerStats.Slide)) },
            { PlayerStats.Jump, Animator.StringToHash(nameof(PlayerStats.Jump)) },
            { PlayerStats.Fall, Animator.StringToHash(nameof(PlayerStats.Fall)) },
            { PlayerStats.Attack1, Animator.StringToHash(nameof(PlayerStats.Attack1)) },
            { PlayerStats.Attack2, Animator.StringToHash(nameof(PlayerStats.Attack2)) },
            { PlayerStats.Skills, Animator.StringToHash(nameof(PlayerStats.Skills)) },
            { PlayerStats.Hurt, Animator.StringToHash(nameof(PlayerStats.Hurt)) },
            { PlayerStats.Death, Animator.StringToHash(nameof(PlayerStats.Death)) }
        };

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
            _runTimeData.currentState = current <= 0 ? PlayerStats.Death : PlayerStats.Hurt;
        }

        private void OnDeathHandler()
        {
            _runTimeData.currentState = PlayerStats.Death;
        }

        public void TriggerJumpPeak()
        {
            OnJumpPeak?.Invoke();
        }

        public void TriggerLanding()
        {
            OnLanding?.Invoke();
        }

        public void TriggerTurnEnd()
        {
            OnTurnEnd?.Invoke();
        }

        public void TriggerSlideEnd()
        {
            OnSlideEnd?.Invoke();
        }

        public void TriggerAttackEnd()
        {
            OnAttackEnd?.Invoke();
        }
    }
}