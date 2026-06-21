using System;
using Script.Interfaces;
using Script.Models;
using UnityEngine;

namespace Script.RunTimeDatas
{
    public class PlayerRunTimeData : MonoBehaviour, IDamageable, IRunTimeData
    {
        [Header("动态数值")] public IDamageable.Stats currentState;
        public float CurrentHealth { get; set; }
        public int Level { get; private set; }
        public float Experience { get; set; }
        public float MaxHealth => ModelManager.PlayerModelSObject.MaxHealth;
        public float Damage => ModelManager.PlayerModelSObject.Damage;
        public float SkillDamage => ModelManager.PlayerModelSObject.SkillDamage;
        public float Defense => ModelManager.PlayerModelSObject.Defense;
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
        private DamageSystem _damageSystem;
        private LevelUpSystem _levelUpSystem;
        public event Action<float, float> OnPlayerHurt;
        public event Action OnPlayerDeath;
        public event Action<int> OnPlayerLevelChanged;
        public event Action<float, float> OnPlayerExperienceChanged;

        private void Awake()
        {
            CurrentHealth = MaxHealth;
            currentState = IDamageable.Stats.Idle;
            _damageSystem = DamageSystem.Instance ?? FindObjectOfType<DamageSystem>();
            _levelUpSystem = FindObjectOfType<LevelUpSystem>();
            if (_levelUpSystem) _levelUpSystem.RegisterPlayer(this);
            else SetProgress(1, 0f);
        }

        public void AttackEnemy(GameObject target, IDamageable.Stats attackerState)
        {
            if (target && _damageSystem) _damageSystem.ApplyDamage(gameObject, target, attackerState);
        }

        public void TakeDamage(float damage)
        {
            if (!_damageSystem) return;
            _damageSystem.ApplyRawDamage(gameObject, damage);
        }

        public void NotifyHurt()
        {
            OnPlayerHurt?.Invoke(CurrentHealth, MaxHealth);
        }

        public void NotifyDeath()
        {
            OnPlayerDeath?.Invoke();
        }

        public void SetProgress(int level, float experience)
        {
            SetLevel(level);
            SetExperience(experience);
        }

        public void SetLevel(int level)
        {
            Level = Mathf.Max(1, level);
            OnPlayerLevelChanged?.Invoke(Level);
        }

        public void SetExperience(float experience)
        {
            Experience = Mathf.Max(0f, experience);
            OnPlayerExperienceChanged?.Invoke(Experience, GetExperienceToNextLevel());
        }

        private float GetExperienceToNextLevel()
        {
            return _levelUpSystem ? _levelUpSystem.GetExperienceToNextLevel(Level) : 0f;
        }
    }
}