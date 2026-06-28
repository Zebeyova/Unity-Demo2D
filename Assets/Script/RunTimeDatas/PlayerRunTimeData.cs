using System;
using System.Collections;
using Script.Interfaces;
using Script.Models;
using UnityEngine;

namespace Script.RunTimeDatas
{
    public class PlayerRunTimeData : MonoBehaviour, IDamageable, IRunTimeData
    {
        [Header("动态数值")] public IDamageable.Stats currentState;
        public float CurrentHealth { get; private set; }
        public int Level { get; private set; }
        public float Experience { get; private set; }
        public float MaxHealth => ModelManager.PlayerModelSObject.MaxHealth;
        public float Damage => ModelManager.PlayerModelSObject.Damage;
        public float SkillDamage => ModelManager.PlayerModelSObject.SkillDamage;
        public float Defense => ModelManager.PlayerModelSObject.Defense;
        public float BaseSpeed => ModelManager.PlayerModelSObject.BaseSpeed;
        public float RunSpeedMultiplier => ModelManager.PlayerModelSObject.runSpeedMultiplier;
        public float SlideSpeedMultiplier => ModelManager.PlayerModelSObject.slideSpeedMultiplier;
        public float SlideCool => ModelManager.PlayerModelSObject.slideCool;
        public float JumpForce => ModelManager.PlayerModelSObject.jumpForce;
        public float HorizontalInputThreshold => ModelManager.PlayerModelSObject.horizontalInputThreshold;
        public bool IsInvincible { get; set; }
        public float InvincibleTime => ModelManager.PlayerModelSObject.InvincibleTime;
        public float ExperienceToNextLevel => GetExperienceToNextLevel();
        private int _killedEnemy;
        private int _allEnemy;
        public event Action<float, float> OnPlayerHurt;
        public event Action OnPlayerDeath;
        public event Action<float, float> OnPlayerExperienceChanged;
        public event Action OnGameOver;

        private void Awake()
        {
            CurrentHealth = MaxHealth;
            currentState = IDamageable.Stats.Idle;
            ApplyProgress(1, 0f);
            _allEnemy = GameObject.FindGameObjectsWithTag("Enemy").Length;
            print(_allEnemy);
        }

        private void OnEnable()
        {
            Events.EventCenter.OnDamageResolved += OnDamageResolved;
            Events.EventCenter.OnEnemyDeath += OnEnemyDeath;
        }

        private void OnDisable()
        {
            Events.EventCenter.OnDamageResolved -= OnDamageResolved;
            Events.EventCenter.OnEnemyDeath -= OnEnemyDeath;
        }

        public void RequestAttack(GameObject target, IDamageable.Stats attackerState)
        {
            if (target)
                Events.EventCenter.TriggerAttackHit(new Events.AttackEventArgs
                {
                    Attacker = gameObject,
                    Target = target,
                    AttackType = attackerState
                });
        }

        public void ApplyDamage(float damage)
        {
            if (IsInvincible || damage <= 0f || CurrentHealth <= 0f) return;

            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
            currentState = CurrentHealth <= 0f ? IDamageable.Stats.Death : IDamageable.Stats.Hurt;

            if (CurrentHealth <= 0f) RaiseDeath();
            else RaiseHurt();

            if (CurrentHealth > 0f) StartCoroutine(InvincibilityRoutine());
        }

        public void ApplyProgress(int level, float experience)
        {
            ApplyLevel(level);
            ApplyExperience(experience);
        }

        public void ApplyLevel(int level)
        {
            Level = Mathf.Max(1, level);
        }

        public void ApplyExperience(float experience)
        {
            Experience = Mathf.Max(0f, experience);
            OnPlayerExperienceChanged?.Invoke(Experience, GetExperienceToNextLevel());
        }

        private void OnDamageResolved(Events.DamageEventArgs args)
        {
            if (args == null || args.Target != gameObject) return;
            ApplyDamage(args.Damage);
        }

        private void RaiseHurt()
        {
            OnPlayerHurt?.Invoke(CurrentHealth, MaxHealth);
        }

        private void RaiseDeath()
        {
            OnPlayerDeath?.Invoke();
        }

        private void OnEnemyDeath(Events.EnemyDeathEventArgs obj)
        {
            _killedEnemy++;
            if (_killedEnemy == _allEnemy) OnGameOver?.Invoke();
        }

        private IEnumerator InvincibilityRoutine()
        {
            IsInvincible = true;
            yield return new WaitForSeconds(InvincibleTime);
            IsInvincible = false;
        }

        private float GetExperienceToNextLevel()
        {
            return LevelUpSystem.Instance.CalculateExperienceToNextLevel(Level);
        }
    }
}