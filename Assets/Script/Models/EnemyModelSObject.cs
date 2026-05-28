using Script.Interfaces;
using UnityEngine;

namespace Script.Models
{
    [CreateAssetMenu(fileName = "NewEnemyProperty", menuName = "GameModel/EnemyModelSObject")]
    public class EnemyModelSObject : ScriptableObject, ICharacterValue
    {
        public float BaseMaxHealth => 25f;
        public float BaseDamage => 2f;
        public float BaseSkillDamage => 0f;
        public float BaseDefense => 10f;
        public float CriticalRate { get; set; }
        public float CriticalDamage { get; set; }
        public float BaseSpeed => 0.5f;
        public float InvincibleTime => 0.6f;
        [Space] public float endError = 0.3f; // 边界误差
        public float distanceFromPlayer = 0.93f;
        public float patrolMaxDistance = 1.5f;
        public float detectSizeX = 4f;
        public float detectSizeY = 1.2f;
    }
}