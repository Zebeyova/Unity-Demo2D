using System;
using Script.Interfaces;
using Script.Models;
using UnityEngine;

namespace Script.RunTimeDatas
{
    public class PlayerRunTimeData : MonoBehaviour,  IDamageable
    {
        [Header("动态数值")] public IDamageable.Stats currentState;
        public float CurrentHealth { get; set; }
        public int Level { get; set; }
        public float Experience { get; set; }
        private float _currentExperience;
        public float MaxHealth => ModelManager.PlayerModelSObject.MaxHealth;
        public float Damage => ModelManager.PlayerModelSObject.Damage;
        public float SkillDamage => ModelManager.PlayerModelSObject.SkillDamage;
        public float Defense => ModelManager.PlayerModelSObject.Defense;
        public float CriticalRate { get; set; }
        public float CriticalDamage { get; set; }
        public float BaseSpeed => ModelManager.PlayerModelSObject.BaseSpeed;
        public float RunSpeedMultiplier => ModelManager.PlayerModelSObject.runSpeedMultiplier;
        public float SlideSpeedMultiplier => ModelManager.PlayerModelSObject.slideSpeedMultiplier;
        public float SlideCool => ModelManager.PlayerModelSObject.slideCool;
        public float JumpForce => ModelManager.PlayerModelSObject.jumpForce;
        public float HorizontalInputThreshold => ModelManager.PlayerModelSObject.horizontalInputThreshold;
        public bool IsInvincible { get; set; }
        public float InvincibleTime => ModelManager.PlayerModelSObject.InvincibleTime;
        private Coroutine _invincibilityCoroutine;
        public event Action<float, float> OnPlayerHurt;
        public event Action OnPlayerDeath;

        private void Awake()
        {
            CurrentHealth = MaxHealth;
            currentState = IDamageable.Stats.Idle;
        }

        public void TakeDamage(float damage)
        {
            if (IsInvincible || damage <= 0 || CurrentHealth <= 0) return;

            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

            if (CurrentHealth <= 0)
                OnPlayerDeath?.Invoke();
            else
                OnPlayerHurt?.Invoke(CurrentHealth, MaxHealth);
        }
    }
}