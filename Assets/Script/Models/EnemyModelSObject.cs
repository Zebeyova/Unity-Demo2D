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
        [Header("敌人属性")] public float maxHealth = 100f;
        public float damage = 10f;
        public float baseSpeed = 0.5f;
        public float invincibilityDuration = 0.6f;
        [Space] public float endError = 0.3f; // 边界误差
        public float distanceFromPlayer = 0.93f;
        public float patrolMaxDistance = 1.5f;
        public float detectSizeX = 4f;
        public float detectSizeY = 1.2f;
    }
}