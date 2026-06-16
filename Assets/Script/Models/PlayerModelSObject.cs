using Script.Interfaces;
using UnityEngine;

namespace Script.Models
{
    [CreateAssetMenu(fileName = "NewPlayerProperty", menuName = "GameModel/PlayerModelSObject")]
    public class PlayerModelSObject : ScriptableObject,ICharacterValue
    {
        public int Level { get; set; }
        public float Experience { get; set; }
        public float MaxHealth => 10f;
        public float Damage => 5f;
        public float SkillDamage => 8f;
        public float Defense => 30f;
        public float CriticalRate { get; set; }
        public float CriticalDamage { get; set; }
        public float BaseSpeed => 2f;
        public float runSpeedMultiplier = 1.5f;
        public float slideSpeedMultiplier = 1.35f;

        public float slideCool = 0.6f;
        public float jumpForce = 10f;
        public float InvincibleTime => 0.2f;
        public float horizontalInputThreshold = 0.01f;
    }
}