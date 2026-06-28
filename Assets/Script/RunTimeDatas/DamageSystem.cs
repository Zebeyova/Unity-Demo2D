using System.Collections.Generic;
using Script.Interfaces;
using UnityEngine;

namespace Script.RunTimeDatas
{
    public class DamageSystem : MonoBehaviour
    {
        private readonly Dictionary<GameObject, DamageCache> _damageCache = new Dictionary<GameObject, DamageCache>();

        private void OnEnable()
        {
            Events.EventCenter.OnAttackHit += HandleAttackHit;
        }

        private void OnDisable()
        {
            Events.EventCenter.OnAttackHit -= HandleAttackHit;
        }

        private void ResolveAttackDamage(GameObject attacker, GameObject defender, IDamageable.Stats damageType)
        {
            if (!attacker || !defender) return;
            var attackCache = GetOrCreateCache(attacker);
            var defenseCache = GetOrCreateCache(defender);
            var attackPower = ReadAttackPower(attackCache, damageType);
            var defense = ReadDefensePower(defenseCache);
            var finalDamage = CalculateFinalDamage(attackPower, defense);
            if (finalDamage > 0f)
                ApplyDamage(GetTargetObject(defenseCache, defender), defenseCache, finalDamage);
        }

        public void Heal(float amount) //回血
        {
            throw new System.NotImplementedException();
        }

        private void HandleAttackHit(Events.AttackEventArgs args)
        {
            if (args == null || !args.Attacker || !args.Target) return;
            ResolveAttackDamage(args.Attacker, args.Target, args.AttackType);
        }

        private float ReadAttackPower(DamageCache attacker, IDamageable.Stats damageType) //拿到基础攻击力
        {
            if (attacker.player)
                return damageType == IDamageable.Stats.Skill ? attacker.player.SkillDamage : attacker.player.Damage;
            if (attacker.enemy)
                return damageType == IDamageable.Stats.Skill ? attacker.enemy.SkillDamage : attacker.enemy.Damage;
            return 0f;
        }

        private float ReadDefensePower(DamageCache defender) //拿到基础防御力
        {
            if (defender.player) return defender.player.Defense;
            return defender.enemy ? defender.enemy.Defense : 0f;
        }

        private float CalculateFinalDamage(float attack, float defense) //最终伤害计算
        {
            return Mathf.Max(0, attack * (1 - defense / 100));
        }

        private void ApplyDamage(GameObject target, DamageCache cachedTarget, float damage) //对目标进行伤害应用
        {
            if (cachedTarget.player || cachedTarget.enemy)
            {
                Events.EventCenter.TriggerDamageResolved(new Events.DamageEventArgs
                {
                    Target = target,
                    Damage = damage
                });
                return;
            }

            var damageable = cachedTarget.damageable ?? target.GetComponent(typeof(IDamageable)) as IDamageable;
            damageable?.ApplyDamage(damage);
            cachedTarget.damageable = damageable;
        }

        private DamageCache GetOrCreateCache(GameObject target)
        {
            if (_damageCache.TryGetValue(target, out var cache) && cache.IsValid) return cache;

            cache = new DamageCache
            {
                player = target.GetComponentInParent<PlayerRunTimeData>() ?? target.GetComponent<PlayerRunTimeData>(),
                enemy = target.GetComponentInParent<EnemyRunTimeData>() ?? target.GetComponent<EnemyRunTimeData>(),
                damageable = target.GetComponentInParent(typeof(IDamageable)) as IDamageable ??
                             target.GetComponent(typeof(IDamageable)) as IDamageable
            };
            _damageCache[target] = cache;
            return cache;
        }

        private GameObject GetTargetObject(DamageCache cache, GameObject fallbackTarget)
        {
            if (cache.player) return cache.player.gameObject;
            return cache.enemy ? cache.enemy.gameObject : fallbackTarget;
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