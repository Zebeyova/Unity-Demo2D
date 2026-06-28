using System;
using System.Collections;
using System.IO;
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
        private string _filePath;
        private int _currentKilledEnemy;
        public int killedEnemy;
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
            _filePath = Application.persistentDataPath + "/killed_enemy.json";
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var data = JsonUtility.FromJson<KilledEnemyData>(json);
                killedEnemy = data?.killedEnemy ?? 0;
            }
            else killedEnemy = 0;
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
                    attacker = gameObject,
                    target = target,
                    attackType = attackerState
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
            if (args == null || args.target != gameObject) return;
            ApplyDamage(args.damage);
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
            killedEnemy++;
            _currentKilledEnemy++;
            var data = new KilledEnemyData { killedEnemy = killedEnemy };
            var json = JsonUtility.ToJson(data);
            File.WriteAllText(_filePath, json);
            Events.EventCenter.TriggerKilledEnemyCountChanged(killedEnemy);
            if (_currentKilledEnemy >= _allEnemy) OnGameOver?.Invoke();
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