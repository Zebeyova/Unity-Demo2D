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
            var damageable = target.GetComponent<IDamageable>();
            damageable?.TakeDamage(damage);
        }
    }
}