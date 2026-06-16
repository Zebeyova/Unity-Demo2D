using System;
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
        public event Action OnEnemyDeath;

        private void Awake()
        {
            CurrentHealth = MaxHealth;
            currentStats = IDamageable.Stats.Idle;
        }

        public void TakeDamage(float damage)
        {
            if (IsInvincible || damage <= 0 || CurrentHealth <= 0) return;

            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

            if (CurrentHealth <= 0)
                OnEnemyDeath?.Invoke();
            else
                OnEnemyHurt?.Invoke(CurrentHealth, MaxHealth);
        }
    }
}