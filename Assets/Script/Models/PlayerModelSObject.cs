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
        Attack1,
        Attack2,
        Skills,
        Hurt,
        Death
    }

    [CreateAssetMenu(fileName = "NewPlayerProperty", menuName = "GameModel/PlayerModelSObject")]
    public class PlayerModelSObject : ScriptableObject
    {
        [Header("玩家属性")] public readonly float maxHealth = 100f;
        public readonly float damage = 15f;
        public readonly float skillDamage = 30f;
        public readonly float baseSpeed = 2f;
        public readonly float runSpeedMultiplier = 1.5f;
        public readonly float slideCool = 0.6f;
        public readonly float jumpForce = 10f;
        [Space] public readonly float horizontalInputThreshold = 0.01f;
        public readonly float invincibleTime = 0.2f; //无敌时间
        public readonly float bufferBarSpeed = 2f;
    }
}