using System;
using System.Collections;
using Script.Interfaces;
using Script.Models;
using UnityEngine;

namespace Script.RunTimeData
{
    public class EnemyRunTimeData : MonoBehaviour, ICharacterValue, IDamageable
    {
        [Header("动态数值")] public ICharacterValue.Stats currentStats;
        public float CurrentHealth { get; set; }
        public float BaseMaxHealth => ModelManager.EnemyModelSObject.BaseMaxHealth;
        public float BaseDamage => ModelManager.EnemyModelSObject.BaseDamage;
        public float BaseSkillDamage => ModelManager.EnemyModelSObject.BaseSkillDamage;
        public float BaseDefense => ModelManager.EnemyModelSObject.BaseDefense;
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
            CurrentHealth = BaseMaxHealth;
            currentStats = ICharacterValue.Stats.Idle;
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
            if (damageSystem) damageSystem.ApplyDamage(gameObject, target, ICharacterValue.Stats.Attack);
        }

        public void TakeDamage(float damage)
        {
            if (IsInvincible || damage <= 0 || CurrentHealth <= 0) return;
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, BaseMaxHealth);
            OnEnemyHurt?.Invoke(CurrentHealth, BaseMaxHealth);
            StartInvincibility();
        }
    }
}