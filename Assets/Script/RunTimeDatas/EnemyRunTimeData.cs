using System;
using System.Collections;
using Script.Interfaces;
using Script.Models;
using Script.Views;
using UnityEngine;

namespace Script.RunTimeDatas
{
    public class EnemyRunTimeData : MonoBehaviour, IDamageable, IRunTimeData
    {
        [Header("动态数值")] public IDamageable.Stats currentStats;
        private IDamageable.Stats _lastStats;
        public float CurrentHealth { get; private set; }
        private int Level { get; set; }
        private float Experience { get; set; }
        public float MaxHealth => ModelManager.EnemyModelSObject.MaxHealth * GetStatMultiplier();
        public float Damage => ModelManager.EnemyModelSObject.Damage * GetStatMultiplier();
        public float SkillDamage => ModelManager.EnemyModelSObject.SkillDamage;
        public float Defense => ModelManager.EnemyModelSObject.Defense * GetStatMultiplier();
        public float BaseSpeed => ModelManager.EnemyModelSObject.BaseSpeed;
        public float EndError => ModelManager.EnemyModelSObject.endError;
        public float DistanceFromPlayer => ModelManager.EnemyModelSObject.distanceFromPlayer;
        public float PatrolMaxDistance => ModelManager.EnemyModelSObject.patrolMaxDistance;
        public bool IsInvincible { get; set; }
        public float InvincibleTime => ModelManager.EnemyModelSObject.InvincibleTime;
        public event Action<float, float> OnEnemyHurt;
        private LevelUpSystem _levelUpSystem;
        private EnemyAnimView _animView;
        private float ExperienceReward => Experience;
        private bool _isDead;

        private void Awake()
        {
            _levelUpSystem = FindObjectOfType<LevelUpSystem>();
            Level = _levelUpSystem ? _levelUpSystem.CurrentLevel : 1;
            Experience = ModelManager.EnemyModelSObject.experienceReward;
            CurrentHealth = MaxHealth;
            currentStats = IDamageable.Stats.Idle;

            _animView = GetComponent<EnemyAnimView>();
            if (_animView) _animView.OnDeathEnd += OnDeathAnimationEnd;
        }

        private void OnEnable()
        {
            Events.EventCenter.OnDamageResolved += OnDamageResolved;
        }

        private void OnDisable()
        {
            Events.EventCenter.OnDamageResolved -= OnDamageResolved;
        }

        private void OnDestroy()
        {
            if (_animView) _animView.OnDeathEnd -= OnDeathAnimationEnd;
        }

        public void ApplyDamage(float damage)
        {
            if (IsInvincible || damage <= 0f || CurrentHealth <= 0f || _isDead) return;
            var previousHealth = CurrentHealth;
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);

            if (CurrentHealth <= 0f)
            {
                _isDead = true;
                currentStats = IDamageable.Stats.Death;
                OnEnemyHurt?.Invoke(0f, MaxHealth);
                return;
            }

            currentStats = IDamageable.Stats.Hurt;
            OnEnemyHurt?.Invoke(CurrentHealth, MaxHealth);

            if (ShouldTriggerInvincibility(previousHealth))
                StartCoroutine(InvincibilityRoutine());
        }

        private void OnDamageResolved(Events.DamageEventArgs args)
        {
            if (args == null || args.target != gameObject) return;
            ApplyDamage(args.damage);
        }

        private bool ShouldTriggerInvincibility(float previousHealth)
        {
            if (MaxHealth <= 0f) return false;
            var previousRatio = previousHealth / MaxHealth;
            var currentRatio = CurrentHealth / MaxHealth;
            return previousRatio > 0.8f && currentRatio <= 0.8f
                   || previousRatio > 0.5f && currentRatio <= 0.5f
                   || previousRatio > 0.3f && currentRatio <= 0.3f;
        }

        private IEnumerator InvincibilityRoutine()
        {
            IsInvincible = true;
            yield return new WaitForSeconds(InvincibleTime);
            IsInvincible = false;
        }

        private float GetStatMultiplier()
        {
            var levelUpSystem = _levelUpSystem
                ? _levelUpSystem
                : LevelUpSystem.Instance ?? FindObjectOfType<LevelUpSystem>();
            return levelUpSystem ? levelUpSystem.CalculateEnemyStatMultiplier(Level) : 1f;
        }

        private void OnDeathAnimationEnd()
        {
            Events.EventCenter.TriggerEnemyDefeated(new Events.EnemyDeathEventArgs
            {
                position = transform.position + new Vector3(0, 0.5f, 0),
                experience = ExperienceReward
            });
        }
    }
}