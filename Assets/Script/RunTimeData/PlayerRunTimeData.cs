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
        public int comboCount;

        public float MaxHealth => ModelManager.PlayerModelSo?.maxHealth ?? 100f;
        public float Damage => ModelManager.PlayerModelSo?.damage ?? 15f;
        public float SkillDamage => ModelManager.PlayerModelSo?.skillDamage ?? 30f;
        public float BaseSpeed => ModelManager.PlayerModelSo?.baseSpeed ?? 2f;
        public float RunSpeedMultiplier => ModelManager.PlayerModelSo?.runSpeedMultiplier ?? 1.5f;
        public float SlideCool => ModelManager.PlayerModelSo?.slideCool ?? 0.6f;
        public float JumpForce => ModelManager.PlayerModelSo?.jumpForce ?? 10f;
        public float HorizontalInputThreshold => ModelManager.PlayerModelSo?.horizontalInputThreshold ?? 0.01f;
        public float InvincibleTime => ModelManager.PlayerModelSo?.invincibleTime ?? 0.2f;
        public float BufferBarSpeed => ModelManager.PlayerModelSo?.bufferBarSpeed ?? 2f;

        public event Action<float, float> OnHurt;
        public event Action OnDeath;
        private bool _invincible;
        private Coroutine _invincibilityCoroutine;

        private void Awake()
        {
            currentHealth = MaxHealth;
            currentState = PlayerStats.Idle;
            comboCount = 0;
        }

        public void PlayerHurt(float damage)
        {
            if (_invincible || damage <= 0 || currentHealth <= 0) return;

            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);
            OnHurt?.Invoke(currentHealth, MaxHealth);

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
            var damage = (attackerState == PlayerStats.Attack1 || attackerState == PlayerStats.Attack2)
                ? Damage
                : SkillDamage;
            enemyData.EnemyHurt(damage);
        }
    }
}