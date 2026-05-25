using Resources;
using UnityEngine;

namespace Script.Player
{
    public class PlayerRunningData : MonoBehaviour
    {
        [Header("当前状态")] public PlayerStats currentState;
        [Space] [Header("基础数值")] public float currentHealth;
        public int comboCount;

        public float MaxHealth => PropertyManager.PlayerProperty?.maxHealth ?? 100f;
        public float Damage => PropertyManager.PlayerProperty?.damage ?? 15f;
        public float SkillDamage => PropertyManager.PlayerProperty?.skillDamage ?? 30f;
        public float BaseSpeed => PropertyManager.PlayerProperty?.baseSpeed ?? 2f;
        public float RunSpeedMultiplier => PropertyManager.PlayerProperty?.runSpeedMultiplier ?? 1.5f;
        public float SlideCool => PropertyManager.PlayerProperty?.slideCool ?? 0.6f;
        public float JumpForce => PropertyManager.PlayerProperty?.jumpForce ?? 10f;
        public float HorizontalInputThreshold => PropertyManager.PlayerProperty?.horizontalInputThreshold ?? 0.01f;
        public float InvincibleTime => PropertyManager.PlayerProperty?.invincibleTime ?? 0.2f;
        public float BufferBarSpeed => PropertyManager.PlayerProperty?.bufferBarSpeed ?? 2f;

        private void Awake()
        {
            currentHealth = MaxHealth;
            currentState = PlayerStats.Idle;
        }
    }
}