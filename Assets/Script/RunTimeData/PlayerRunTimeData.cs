using System;
using System.Collections;
using Script.Models;
using UnityEngine;

namespace Script.RunTimeData
{
    public class PlayerRunTimeData : MonoBehaviour
    {
        [Header("动态数值")] public PlayerStats currentState;
        public float currentHealth;

        public float MaxHealth => ModelManager.PlayerModelSObject?.maxHealth ?? 100f;
        private float Damage => ModelManager.PlayerModelSObject?.damage ?? 15f;
        private float SkillDamage => ModelManager.PlayerModelSObject?.skillDamage ?? 30f;
        public float BaseSpeed => ModelManager.PlayerModelSObject?.baseSpeed ?? 2f;
        public float RunSpeedMultiplier => ModelManager.PlayerModelSObject?.runSpeedMultiplier ?? 1.5f;
        public float SlideSpeedMultiplier => ModelManager.PlayerModelSObject?.slideSpeedMultiplier ?? 1.5f;
        public float SlideCool => ModelManager.PlayerModelSObject?.slideCool ?? 0.6f;
        public float JumpForce => ModelManager.PlayerModelSObject?.jumpForce ?? 10f;
        public float HorizontalInputThreshold => ModelManager.PlayerModelSObject?.horizontalInputThreshold ?? 0.01f;
        private float InvincibleTime => ModelManager.PlayerModelSObject?.invincibleTime ?? 0.2f;
        public float BufferBarSpeed => ModelManager.PlayerModelSObject?.bufferBarSpeed ?? 2f;

        public event Action<float, float> OnPlayerHurt;
        public event Action OnDeath;
        private bool _invincible;
        private Coroutine _invincibilityCoroutine;

        private void Awake()
        {
            currentHealth = MaxHealth;
            currentState = PlayerStats.Idle;
        }

        public void PlayerHurt(float damage)
        {
            if (_invincible || damage <= 0 || currentHealth <= 0) return;

            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);
            OnPlayerHurt?.Invoke(currentHealth, MaxHealth);

            if (currentHealth <= 0)
                OnDeath?.Invoke();
            else
                StartInvincibility();
        }

        private void StartInvincibility()
        {
            if (_invincibilityCoroutine != null) StopCoroutine(_invincibilityCoroutine);
            _invincibilityCoroutine = StartCoroutine(InvincibilityRoutine());
        }

        private IEnumerator InvincibilityRoutine()
        {
            _invincible = true;
            yield return new WaitForSeconds(InvincibleTime);
            _invincible = false;
        }

        public void AttackEnemy(GameObject target, PlayerStats attackerState)
        {
            if (!target) return;
            var enemyData = target.GetComponent<EnemyRunTimeData>();
            if (!enemyData || !enemyData.isActiveAndEnabled) return;
            var damage = attackerState == PlayerStats.Attack1 || attackerState == PlayerStats.Attack2
                ? Damage
                : SkillDamage;
            enemyData.EnemyHurt(damage);
        }
    }
}