using System.Collections.Generic;
using Script.Interfaces;
using UnityEngine;

namespace Script.RunTimeDatas
{
    public class DamageSystem : MonoBehaviour
    {
        [SerializeField] [Min(0f)] private float playerInvincibleDuration = 0.5f;
        [SerializeField] [Min(0f)] private float enemyInvincibleDuration = 1f;

        private readonly Dictionary<GameObject, DamageCache> _damageCache = new Dictionary<GameObject, DamageCache>();

        private void OnEnable()
        {
            Events.EventCenter.OnAttackHit += OnAttackHitHandler;
        }

        private void OnDisable()
        {
            Events.EventCenter.OnAttackHit -= OnAttackHitHandler;
        }

        public void ApplyDamage(GameObject attacker, GameObject defender, IDamageable.Stats damageType)
        {
            if (!attacker || !defender) return;
            var attackCache = GetOrCreateCache(attacker);
            var defenseCache = GetOrCreateCache(defender);
            var attackPower = GetAttackPower(attackCache, damageType);
            var defense = GetDefensePower(defenseCache);
            var finalDamage = CalculateDamage(attackPower, defense);
            if (finalDamage > 0) ApplyDamageToTarget(defenseCache, defender, finalDamage);
        }

        public void ApplyRawDamage(GameObject target, float damage)
        {
            if (!target || damage <= 0) return;
            ApplyDamageToTarget(GetOrCreateCache(target), target, damage);
        }

        public void Heal(float amount) //回血
        {
            throw new System.NotImplementedException();
        }

        private void OnAttackHitHandler(Events.AttackEventArgs args)
        {
            if (args == null || !args.attacker || !args.target) return;
            ApplyDamage(args.attacker, args.target, args.attackType);
        }

        private float GetAttackPower(DamageCache attacker, IDamageable.Stats damageType) //拿到基础攻击力
        {
            if (attacker.player)
                return damageType == IDamageable.Stats.Skill ? attacker.player.SkillDamage : attacker.player.Damage;
            if (attacker.enemy)
                return damageType == IDamageable.Stats.Skill ? attacker.enemy.SkillDamage : attacker.enemy.Damage;
            return 0f;
        }

        private float GetDefensePower(DamageCache defender) //拿到基础防御力
        {
            if (defender.player) return defender.player.Defense;
            return defender.enemy ? defender.enemy.Defense : 0f;
        }

        private float CalculateDamage(float attack, float defense) //最终伤害计算
        {
            return Mathf.Max(0, attack * (1 - defense / 100));
        }

        private void ApplyDamageToTarget(DamageCache cachedTarget, GameObject target, float damage) //对目标进行伤害应用
        {
            if (cachedTarget.player)
            {
                ApplyDamageToPlayer(cachedTarget.player, damage);
                return;
            }

            if (cachedTarget.enemy)
            {
                ApplyDamageToEnemy(cachedTarget.enemy, damage);
                return;
            }

            var damageable = cachedTarget.damageable ?? target.GetComponent(typeof(IDamageable)) as IDamageable;
            damageable?.TakeDamage(damage);
            cachedTarget.damageable = damageable;
        }

        private DamageCache GetOrCreateCache(GameObject target)
        {
            if (_damageCache.TryGetValue(target, out var cache) && cache.IsValid) return cache;

            cache = new DamageCache
            {
                player = target.GetComponent<PlayerRunTimeData>(),
                enemy = target.GetComponent<EnemyRunTimeData>(),
                damageable = target.GetComponent(typeof(IDamageable)) as IDamageable
            };
            _damageCache[target] = cache;
            return cache;
        }

        private void ApplyDamageToPlayer(PlayerRunTimeData player, float damage)
        {
            if (player.IsInvincible || damage <= 0 || player.CurrentHealth <= 0) return;
            player.CurrentHealth = Mathf.Clamp(player.CurrentHealth - damage, 0, player.MaxHealth);
            player.currentState = player.CurrentHealth <= 0 ? IDamageable.Stats.Death : IDamageable.Stats.Hurt;

            if (player.CurrentHealth <= 0) player.NotifyDeath();
            else player.NotifyHurt();
            if (player.CurrentHealth > 0) StartCoroutine(InvincibilityRoutine(player, playerInvincibleDuration));
        }

        private void ApplyDamageToEnemy(EnemyRunTimeData enemy, float damage)
        {
            if (enemy.IsInvincible || damage <= 0 || enemy.CurrentHealth <= 0) return;

            var previousHealth = enemy.CurrentHealth;
            enemy.CurrentHealth = Mathf.Clamp(enemy.CurrentHealth - damage, 0, enemy.MaxHealth);
            enemy.currentStats = enemy.CurrentHealth <= 0 ? IDamageable.Stats.Death : IDamageable.Stats.Hurt;
            enemy.NotifyHurt();

            if (enemy.CurrentHealth > 0 && TryTriggerEnemyInvincibility(enemy, previousHealth))
                StartCoroutine(InvincibilityRoutine(enemy, enemyInvincibleDuration));
        }

        private bool TryTriggerEnemyInvincibility(EnemyRunTimeData enemy, float previousHealth)
        {
            var cache = GetOrCreateCache(enemy.gameObject);
            var maxHealth = enemy.MaxHealth;
            var previousRatio = previousHealth / maxHealth;
            var currentRatio = enemy.CurrentHealth / maxHealth;
            var triggered = false;

            if (!cache.enemyInvincibilityTriggered80 &&
                previousRatio > 0.8f && currentRatio <= 0.8f)
            {
                cache.enemyInvincibilityTriggered80 = true;
                triggered = true;
            }

            if (!cache.enemyInvincibilityTriggered50 &&
                previousRatio > 0.5f && currentRatio <= 0.5f)
            {
                cache.enemyInvincibilityTriggered50 = true;
                triggered = true;
            }

            if (!cache.enemyInvincibilityTriggered30 &&
                previousRatio > 0.3f && currentRatio <= 0.3f)
            {
                cache.enemyInvincibilityTriggered30 = true;
                triggered = true;
            }

            return triggered;
        }

        private System.Collections.IEnumerator InvincibilityRoutine(IRunTimeData target, float duration)
        {
            target.IsInvincible = true;
            yield return new WaitForSeconds(duration);
            target.IsInvincible = false;
        }

        private sealed class DamageCache
        {
            public PlayerRunTimeData player;
            public EnemyRunTimeData enemy;
            public IDamageable damageable;
            public bool enemyInvincibilityTriggered80;
            public bool enemyInvincibilityTriggered50;
            public bool enemyInvincibilityTriggered30;

            public bool IsValid => player || enemy || damageable != null;
        }
    }
}