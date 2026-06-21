using System.Collections.Generic;
using Script.Interfaces;
using UnityEngine;

namespace Script.RunTimeDatas
{
    public class DamageSystem : MonoBehaviour
    {
        private readonly Dictionary<GameObject, DamageCache> _damageCache = new Dictionary<GameObject, DamageCache>();

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

        private float GetAttackPower(DamageCache attacker, IDamageable.Stats damageType) //拿到基础攻击力
        {
            if (attacker.player) return damageType == IDamageable.Stats.Skill ? attacker.player.SkillDamage : attacker.player.Damage;
            if (attacker.enemy) return damageType == IDamageable.Stats.Skill ? attacker.enemy.SkillDamage : attacker.enemy.Damage;
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
            if (player.CurrentHealth > 0) StartCoroutine(InvincibilityRoutine(player));
        }

        private void ApplyDamageToEnemy(EnemyRunTimeData enemy, float damage)
        {
            if (enemy.IsInvincible || damage <= 0 || enemy.CurrentHealth <= 0) return;
            enemy.CurrentHealth = Mathf.Clamp(enemy.CurrentHealth - damage, 0, enemy.MaxHealth);
            enemy.currentStats = enemy.CurrentHealth <= 0 ? IDamageable.Stats.Death : IDamageable.Stats.Hurt;
            enemy.NotifyHurt();

            if (enemy.CurrentHealth > 0) StartCoroutine(InvincibilityRoutine(enemy));
        }

        private System.Collections.IEnumerator InvincibilityRoutine<T>(T target)
            where T : class, IRunTimeData //公共无敌时间协程
        {
            target.IsInvincible = true;
            yield return new WaitForSeconds(target.InvincibleTime);
            target.IsInvincible = false;
        }

        private sealed class DamageCache
        {
            public PlayerRunTimeData player;
            public EnemyRunTimeData enemy;
            public IDamageable damageable;

            public bool IsValid => player || enemy || damageable != null;
        }
    }
}