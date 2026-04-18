using UnityEngine;

namespace Script.Enemy
{
    public class EnemyProperties : MonoBehaviour
    {
        [Header("敌人属性")] public float maxHealth = 100f;
        public float damage = 10f;
        public float baseSpeed = 0.5f; //基础速度
        public float attackCoolDown = 1f;
        [Space] public float endError = 0.3f; //边界误差
        public float distanceFromPlayer = 0.93f;
        public float patrolMaxDistance = 2f;
    }
}