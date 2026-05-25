using UnityEngine;

namespace Script.Models
{
    public enum EnemyState
    {
        Idle,
        Walk,
        Attack,
        Hurt,
        Die
    }

    [CreateAssetMenu(fileName = "NewEnemyProperty", menuName = "GameModel/EnemyModelSObject")]
    public class EnemyModelSObject : ScriptableObject
    {
        [Header("敌人属性")] public readonly float maxHealth = 100f;
        public readonly float damage = 10f;
        public readonly float baseSpeed = 0.5f;
        public readonly float attackCoolDown = 1f; // 攻击冷却
        [Space] public readonly float endError = 0.3f; // 边界误差
        public readonly float distanceFromPlayer = 0.93f;
        public readonly float patrolMaxDistance = 2f;
    }
}