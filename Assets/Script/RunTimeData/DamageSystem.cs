using Script.Interfaces;
using UnityEngine;

namespace Script.RunTimeData
{
    public class DamageSystem : MonoBehaviour
    {
        public void ApplyDamage(GameObject attacker, GameObject defender, ICharacterValue.Stats damageType)
        {
            if (!attacker || !defender) return;
            var attackPower = GetAttackPower(attacker, damageType);
            var defense = GetDefensePower(defender);
            var finalDamage = CalculateDamage(attackPower, defense);
            if (finalDamage > 0) ApplyDamageToTarget(defender, finalDamage);
        }

        public void TakeDamageOrDeath(GameObject injuredParty, float damage)
        {
        }

        public void Heal(float amount)
        {
            throw new System.NotImplementedException();
        }

        private float GetAttackPower(GameObject attacker, ICharacterValue.Stats damageType)
        {
            var player = attacker.GetComponent<PlayerRunTimeData>();
            if (player) return damageType == ICharacterValue.Stats.Skill ? player.BaseSkillDamage : player.BaseDamage;
            var enemy = attacker.GetComponent<EnemyRunTimeData>();
            if (enemy) return damageType == ICharacterValue.Stats.Skill ? enemy.BaseSkillDamage : enemy.BaseDamage;
            return 0f;
        }

        private float GetDefensePower(GameObject defender)
        {
            var player = defender.GetComponent<PlayerRunTimeData>();
            if (player) return player.BaseDefense;
            var enemy = defender.GetComponent<EnemyRunTimeData>();
            return enemy ? enemy.BaseDefense : 0f;
        }

        private float CalculateDamage(float attack, float defense)
        {
            return Mathf.Max(0, attack * (1 - defense / 100));
        }

        private void ApplyDamageToTarget(GameObject target, float damage)
        {
            var damageable = target.GetComponent<IDamageable>();
            damageable?.TakeDamage(damage);
        }
    }
}