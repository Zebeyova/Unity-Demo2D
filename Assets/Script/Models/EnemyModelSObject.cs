using UnityEngine;
using UnityEngine.Serialization;

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
        public float attackCoolDown = 1.5f; // 攻击冷却
        [Space] public float endError = 0.3f; // 边界误差
        public float distanceFromPlayer = 0.93f;
        public float patrolMaxDistance = 1.5f;
        // public float exitDelayTimer = 1f; //玩家退出延迟检测
        public float detectSizeX = 4f;
        public float detectSizeY = 1.2f;
    }
}