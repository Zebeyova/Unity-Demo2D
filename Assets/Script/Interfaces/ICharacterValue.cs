using System;

namespace Script.Interfaces
{
    public interface ICharacterValue
    {
        enum Stats
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
            Attack,
            Attack2,
            Skill,
            Hurt,
            Death
        }

        float BaseMaxHealth { get; } //基础最大生命值
        float BaseDamage { get; } //基本伤害
        float BaseSkillDamage { get; } //技能伤害
        float BaseDefense { get; } //基础防御力
        float CriticalRate { get; set; } //暴击率
        float CriticalDamage { get; set; } //暴击率
        float BaseSpeed { get; } //移动速度
        float InvincibleTime { get; } //无敌时间
    }
}