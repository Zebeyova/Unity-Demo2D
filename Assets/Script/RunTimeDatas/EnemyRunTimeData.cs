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
        public float MaxHealth => ModelManager.EnemyModelSObject.MaxHealth * GetStatMultiplier();
        public float Damage => ModelManager.EnemyModelSObject.Damage * GetStatMultiplier();
        public float SkillDamage => ModelManager.EnemyModelSObject.SkillDamage;
        public float Defense => ModelManager.EnemyModelSObject.Defense * GetStatMultiplier();
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
        private DamageSystem _damageSystem;
        private LevelUpSystem _levelUpSystem;
        public event Action<float, float> OnEnemyHurt;
        public event Action OnEnemyDeath;
        public float ExperienceReward => Experience;

        private void Awake()
        {
            _levelUpSystem = FindObjectOfType<LevelUpSystem>();
            Level = _levelUpSystem ? _levelUpSystem.CurrentLevel : 1;
            Experience = ModelManager.EnemyModelSObject.experienceReward;
            CurrentHealth = MaxHealth;
            currentStats = IDamageable.Stats.Idle;
            _damageSystem = DamageSystem.Instance ?? FindObjectOfType<DamageSystem>();
        }

        public void TakeDamage(float damage)
        {
            if (!_damageSystem) return;
            _damageSystem.ApplyRawDamage(gameObject, damage);
        }

        public void NotifyHurt()
        {
            OnEnemyHurt?.Invoke(CurrentHealth, MaxHealth);
        }

        public void NotifyDeath()
        {
            OnEnemyDeath?.Invoke();
        }

        private float GetStatMultiplier()
        {
            return _levelUpSystem ? _levelUpSystem.GetEnemyStatMultiplier(Level) : 1f;
        }
    }
}