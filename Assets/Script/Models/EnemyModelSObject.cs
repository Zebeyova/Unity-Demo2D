using UnityEngine;

namespace Script.Models
{
    [CreateAssetMenu(fileName = "NewEnemyProperty", menuName = "GameModel/EnemyModelSObject")]
    public class EnemyModelSObject : ScriptableObject
    {
        public int Level { get; set; }
        [Header("基础奖励")] [Min(0f)] public float experienceReward = 5f;
        public float MaxHealth => 25f;
        public float Damage => 2f;
        public float SkillDamage => 0f;
        public float Defense => 10f;
        public float CriticalRate { get; set; }
        public float CriticalDamage { get; set; }
        public float BaseSpeed => 0.5f;
        public float InvincibleTime => 0.6f;
        [Space] public float endError = 0.3f; // 边界误差
        public float distanceFromPlayer = 0.93f;
        public float patrolMaxDistance = 1.5f;
    }
}