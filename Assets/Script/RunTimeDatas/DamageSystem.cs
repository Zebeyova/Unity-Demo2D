using Script.Interfaces;
using UnityEngine;

namespace Script.RunTimeDatas
{
    public class DamageSystem : MonoBehaviour
    {
        public void ApplyDamage(GameObject attacker, GameObject defender, IDamageable.Stats damageType)
        {
            if (!attacker || !defender) return;
            var attackPower = GetAttackPower(attacker, damageType);
            var defense = GetDefensePower(defender);
            var finalDamage = CalculateDamage(attackPower, defense);
            if (finalDamage > 0) ApplyDamageToTarget(defender, finalDamage);
        }

        public void ApplyRawDamage(GameObject target, float damage)
        {
            if (!target || damage <= 0) return;
            ApplyDamageToTarget(target, damage);
        }

        public void Heal(float amount) //回血
        {
            throw new System.NotImplementedException();
        }

        private float GetAttackPower(GameObject attacker, IDamageable.Stats damageType) //拿到基础攻击力
        {
            var player = attacker.GetComponent<PlayerRunTimeData>();
            if (player) return damageType == IDamageable.Stats.Skill ? player.SkillDamage : player.Damage;
            var enemy = attacker.GetComponent<EnemyRunTimeData>();
            if (enemy) return damageType == IDamageable.Stats.Skill ? enemy.SkillDamage : enemy.Damage;
            return 0f;
        }

        private float GetDefensePower(GameObject defender) //拿到基础防御力
        {
            var player = defender.GetComponent<PlayerRunTimeData>();
            if (player) return player.Defense;
            var enemy = defender.GetComponent<EnemyRunTimeData>();
            return enemy ? enemy.Defense : 0f;
        }

        private float CalculateDamage(float attack, float defense) //最终伤害计算
        {
            return Mathf.Max(0, attack * (1 - defense / 100));
        }

        private void ApplyDamageToTarget(GameObject target, float damage) //对目标进行伤害应用
        {
            var player = target.GetComponent<PlayerRunTimeData>();
            if (player)
            {
                ApplyDamageToPlayer(player, damage);
                return;
            }

            var enemy = target.GetComponent<EnemyRunTimeData>();
            if (enemy)
            {
                ApplyDamageToEnemy(enemy, damage);
                return;
            }

            var damageable = target.GetComponent<IDamageable>();
            damageable?.TakeDamage(damage);
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
    }
}