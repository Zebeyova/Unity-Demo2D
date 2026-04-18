using UnityEngine;

namespace Script.Player
{
    public class PlayerProperties : MonoBehaviour
    {
        [Header("玩家属性")] public float maxHealth = 100f;
        public float damage = 15f;
        public float skillDamage = 30f;
        public float baseSpeed = 2f;
        public float runSpeedMultiplier = 1.5f;
        public float slideCool = 0.6f;
        public float jumpForce = 10f;
        [Space] public float horizontalInputThreshold = 0.01f;
        public float invincibleTime = 0.2f; //无敌时间
        public float bufferBarSpeed = 2f;
    }
}   