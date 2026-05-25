using Resources;
using UnityEngine;

namespace Script.Enemy
{


    public class EnemyRunningData : MonoBehaviour
    {
        [Header("当前状态")] public EnemyState currentState;

        [Header("基础数值")] public float currentHealth;
        public bool isAttackingCooldown;

        public float MaxHealth => PropertyManager.EnemyProperty?.maxHealth ?? 100f;
        public float Damage => PropertyManager.EnemyProperty?.damage ?? 10f;
        public float BaseSpeed => PropertyManager.EnemyProperty?.baseSpeed ?? 0.5f;
        public float AttackCoolDown => PropertyManager.EnemyProperty?.attackCoolDown ?? 1f;
        public float EndError => PropertyManager.EnemyProperty?.endError ?? 0.3f;
        public float DistanceFromPlayer => PropertyManager.EnemyProperty?.distanceFromPlayer ?? 0.93f;
        public float PatrolMaxDistance => PropertyManager.EnemyProperty?.patrolMaxDistance ?? 2f;

        private void Awake()
        {
            currentHealth = MaxHealth;
            currentState = EnemyState.Idle;
        }
    }
}