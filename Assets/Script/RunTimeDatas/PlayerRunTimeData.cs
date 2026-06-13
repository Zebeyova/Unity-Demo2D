using System;
using System.Collections;
using Script.Interfaces;
using Script.Models;
using UnityEngine;

namespace Script.RunTimeDatas
{
    public class PlayerRunTimeData : MonoBehaviour, ICharacterValue, IDamageable
    {
        [Header("动态数值")] public ICharacterValue.Stats currentState;
        public float CurrentHealth { get; set; }
        public float BaseMaxHealth => ModelManager.PlayerModelSObject.BaseMaxHealth;
        public float BaseDamage => ModelManager.PlayerModelSObject.BaseDamage;
        public float BaseSkillDamage => ModelManager.PlayerModelSObject.BaseSkillDamage;
        public float BaseDefense => ModelManager.PlayerModelSObject.BaseDefense;
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
            CurrentHealth = BaseMaxHealth;
            currentState = ICharacterValue.Stats.Idle;
        }

        private void StartInvincibility()
        {
            if (_invincibilityCoroutine != null) StopCoroutine(_invincibilityCoroutine);
            _invincibilityCoroutine = StartCoroutine(InvincibilityRoutine());
        }

        private IEnumerator InvincibilityRoutine()
        {
            IsInvincible = true;
            yield return new WaitForSeconds(InvincibleTime);
            IsInvincible = false;
        }

        public void AttackEnemy(GameObject target, ICharacterValue.Stats attackerState)
        {
            if (!target) return;
            var damageSystem = FindObjectOfType<DamageSystem>();
            if (damageSystem) damageSystem.ApplyDamage(gameObject, target, attackerState);
        }

        public void TakeDamage(float damage)
        {
            if (IsInvincible || damage <= 0 || CurrentHealth <= 0) return;
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, BaseMaxHealth);
            OnPlayerHurt?.Invoke(CurrentHealth, BaseMaxHealth);
            if(CurrentHealth <= 0) OnPlayerDeath?.Invoke();
            StartInvincibility();
        }
    }
}