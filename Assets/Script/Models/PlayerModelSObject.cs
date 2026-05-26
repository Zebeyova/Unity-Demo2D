using UnityEngine;

namespace Script.Models
{
    public enum PlayerStats
    {
        Idle,
        Walk,
        WalkTurn,
        Run,
        RunTurn,
        Slide,
        Jump,
        Fall,
        FallLoop,
        Attack1,
        Attack2,
        Skills,
        Hurt,
        Death
    }

    [CreateAssetMenu(fileName = "NewPlayerProperty", menuName = "GameModel/PlayerModelSObject")]
    public class PlayerModelSObject : ScriptableObject
    {
        [Header("玩家属性")] public  float maxHealth = 100f;
        public float damage = 15f;
        public float skillDamage = 30f;
        public float baseSpeed = 2f;
        public float runSpeedMultiplier = 1.5f;
        public float slideSpeedMultiplier = 1.35f;
        public float slideCool = 0.6f;
        public float jumpForce = 10f;
        [Space] public float horizontalInputThreshold = 0.01f;
        public float invincibleTime = 0.2f; //无敌时间
        public float bufferBarSpeed = 2f;
    }
}