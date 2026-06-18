using System;
using System.Collections;
using Script.Interfaces;
using Script.Models;
using UnityEngine;

namespace Script.RunTimeDatas
{
    public class EnemyRunTimeData : MonoBehaviour, IDamageable
    {
        [Header("动态数值")] public IDamageable.Stats currentStats;
        public float CurrentHealth { get; set; }
        public int Level { get; set; }
        public float Experience { get; set; }
        public float MaxHealth => ModelManager.EnemyModelSObject.MaxHealth;
        public float Damage => ModelManager.EnemyModelSObject.Damage;
        public float SkillDamage => ModelManager.EnemyModelSObject.SkillDamage;
        public float Defense => ModelManager.EnemyModelSObject.Defense;
        public float CriticalRate { get; set; }
        public float CriticalDamage { get; set; }
        public float BaseSpeed => ModelManager.EnemyModelSObject.BaseSpeed;
        public float EndError => ModelManager.EnemyModelSObject.endError;
        public float DistanceFromPlayer => ModelManager.EnemyModelSObject.distanceFromPlayer;
        public float PatrolMaxDistance => ModelManager.EnemyModelSObject.patrolMaxDistance;
        public float DetectSizeX => ModelManager.EnemyModelSObject.detectSizeX;
        public float DetectSizeY => ModelManager.EnemyModelSObject.detectSizeY;
        public bool IsInvincible { get; set; }
        public float InvincibleTime => ModelManager.EnemyModelSObject.InvincibleTime;
        private Coroutine _invincibilityCoroutine;
        public event Action<float, float> OnEnemyHurt;

        private void Awake()
        {
            CurrentHealth = MaxHealth;
            currentStats = IDamageable.Stats.Idle;
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

        public void AttackPlayer(GameObject target)
        {
            if (!target) return;
            var damageSystem = FindObjectOfType<DamageSystem>();
            if (damageSystem) damageSystem.ApplyDamage(gameObject, target, IDamageable.Stats.Attack);
        }

        public void TakeDamage(float damage)
        {
            if (IsInvincible || damage <= 0 || CurrentHealth <= 0) return;
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
            OnEnemyHurt?.Invoke(CurrentHealth, MaxHealth);
            StartInvincibility();
        }
    }
}