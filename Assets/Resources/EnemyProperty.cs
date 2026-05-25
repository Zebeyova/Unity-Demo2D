using UnityEngine;

namespace Resources
{
    public enum EnemyState
    {
        Idle,
        Walk,
        Attack,
        Hurt,
        Die
    }

    [CreateAssetMenu(fileName = "NewEnemyProperty", menuName = "Game/EnemyProperty")]
    public class EnemyProperty : ScriptableObject
    {
        [Header("敌人属性")] public float maxHealth = 100f;
        public float damage = 10f;
        public float baseSpeed = 0.5f;
        public float attackCoolDown = 1f; // 攻击冷却
        [Space] public float endError = 0.3f; // 边界误差
        public float distanceFromPlayer = 0.93f;
        public float patrolMaxDistance = 2f;
    }
}