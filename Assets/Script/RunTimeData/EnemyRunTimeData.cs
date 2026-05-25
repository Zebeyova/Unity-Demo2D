using System;
using System.Collections;
using Script.Models;
using UnityEngine;

namespace Script.RunTimeData
{
    public class EnemyRunTimeData : MonoBehaviour
    {
        [Header("动态数值")] public EnemyState currentState;
        public float currentHealth;
        public float MaxHealth => ModelManager.EnemyModelSo?.maxHealth ?? 100f;
        public float Damage => ModelManager.EnemyModelSo?.damage ?? 10f;
        public float BaseSpeed => ModelManager.EnemyModelSo?.baseSpeed ?? 0.5f;
        public float AttackCoolDown => ModelManager.EnemyModelSo?.attackCoolDown ?? 1f;
        public float EndError => ModelManager.EnemyModelSo?.endError ?? 0.3f;
        public float DistanceFromPlayer => ModelManager.EnemyModelSo?.distanceFromPlayer ?? 0.93f;
        public float PatrolMaxDistance => ModelManager.EnemyModelSo?.patrolMaxDistance ?? 2f;
        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;

        private bool _invincible;
        private Coroutine _invincibilityCoroutine;

        private void Awake()
        {
            currentHealth = MaxHealth;
            currentState = EnemyState.Idle;
        }

        public void EnemyHurt(float damage)
        {
            if (_invincible || damage <= 0 || currentHealth <= 0) return;

            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);
            OnHealthChanged?.Invoke(currentHealth, MaxHealth);

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
            yield return new WaitForSeconds(0.2f); // 敌人无敌时间固定 0.2 秒，可改为从 Model 读取
            _invincible = false;
        }
        public void AttackPlayer(GameObject target, PlayerStats attackerState)
        {
            if (!target) return;
            var playerData = target.GetComponent<PlayerRunTimeData>();
            if (!playerData || !playerData.isActiveAndEnabled) return;
            playerData.PlayerHurt(Damage);
        }
    }
}