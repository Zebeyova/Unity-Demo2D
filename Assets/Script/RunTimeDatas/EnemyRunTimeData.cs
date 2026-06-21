using System;
using Script.Interfaces;
using Script.Models;
using UnityEngine;

namespace Script.RunTimeDatas
{
    public class EnemyRunTimeData : MonoBehaviour, IDamageable, IRunTimeData
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
        public event Action<float, float> OnEnemyHurt;

        private void Awake()
        {
            CurrentHealth = MaxHealth;
            currentStats = IDamageable.Stats.Idle;
        }

        public void TakeDamage(float damage)
        {
            var damageSystem = FindObjectOfType<DamageSystem>();
            if (!damageSystem)
            {
                Debug.LogError($"{nameof(DamageSystem)} not found, cannot apply damage.");
                return;
            }

            damageSystem.ApplyRawDamage(gameObject, damage);
        }

        public void NotifyHurt()
        {
            OnEnemyHurt?.Invoke(CurrentHealth, MaxHealth);
        }
    }
}