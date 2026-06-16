using Script.Interfaces;
using UnityEngine;
using System.Collections;

namespace Script.RunTimeDatas
{
    public class DamageSystem : MonoBehaviour
    {
        private void OnEnable()
        {
            Events.EventCenter.OnAttackHit += ApplyDamage;
        }

        private void OnDisable()
        {
            Events.EventCenter.OnAttackHit -= ApplyDamage;
        }

        private void ApplyDamage(Events.AttackEventArgs args)
        {
            if (!args.attacker || !args.target) return;
            if (GetInvincibleInfo(args.target, out var isInvincible, out var invincibleTime))
            {
                if (isInvincible) return; // 无敌时免疫伤害
            }

            // 计算攻击力、防御力、最终伤害
            var attackPower = GetAttackPower(args.attacker, args.attackType);
            var defense = GetDefensePower(args.target);
            var finalDamage = CalculateDamage(attackPower, defense);

            if (!(finalDamage > 0)) return;
            ApplyDamageToTarget(args.target.GetComponent<IDamageable>(), finalDamage);
            if (invincibleTime > 0)
                StartCoroutine(InvincibilityCoroutine(args.target, invincibleTime));
        }

        private bool GetInvincibleInfo(GameObject target, out bool isInvincible, out float invincibleTime)
        {
            isInvincible = false;
            invincibleTime = 0f;

            var player = target.GetComponent<PlayerRunTimeData>();
            if (player)
            {
                isInvincible = player.IsInvincible;
                invincibleTime = player.InvincibleTime;
                return true;
            }

            var enemy = target.GetComponent<EnemyRunTimeData>();
            if (!enemy) return false;
            isInvincible = enemy.IsInvincible;
            invincibleTime = enemy.InvincibleTime;
            return true;
        }

        private void SetTargetInvincible(GameObject target, bool invincible)
        {
            var player = target.GetComponent<PlayerRunTimeData>();
            if (player)
            {
                player.IsInvincible = invincible;
                return;
            }

            var enemy = target.GetComponent<EnemyRunTimeData>();
            if (enemy)
            {
                enemy.IsInvincible = invincible;
            }
        }

        private IEnumerator InvincibilityCoroutine(GameObject target, float duration)
        {
            SetTargetInvincible(target, true);
            yield return new WaitForSeconds(duration);
            SetTargetInvincible(target, false);
        }

        private float GetAttackPower(GameObject attacker, IDamageable.Stats damageType)
        {
            var player = attacker.GetComponent<PlayerRunTimeData>();
            if (player) return damageType == IDamageable.Stats.Skill ? player.SkillDamage : player.Damage;
            var enemy = attacker.GetComponent<EnemyRunTimeData>();
            if (enemy) return damageType == IDamageable.Stats.Skill ? enemy.SkillDamage : enemy.Damage;
            return 0f;
        }

        private float GetDefensePower(GameObject defender)
        {
            var player = defender.GetComponent<PlayerRunTimeData>();
            if (player) return player.Defense;
            var enemy = defender.GetComponent<EnemyRunTimeData>();
            return enemy ? enemy.Defense : 0f;
        }

        private float CalculateDamage(float attack, float defense)
        {
            return Mathf.Max(0, attack * (1 - defense / 100));
        }

        private void ApplyDamageToTarget(IDamageable target, float damage)
        {
            target?.TakeDamage(damage);
        }
    }
}