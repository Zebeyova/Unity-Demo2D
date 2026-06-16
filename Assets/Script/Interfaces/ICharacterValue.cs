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

        int Level { get; set; } //等级
        float Experience { get; set; } //经验
        float MaxHealth { get; } //基础最大生命值
        float Damage { get; } //基本伤害
        float SkillDamage { get; } //技能伤害
        float Defense { get; } //基础防御力
        float CriticalRate { get; set; } //暴击率
        float CriticalDamage { get; set; } //暴击率
        float BaseSpeed { get; } //移动速度
        float InvincibleTime { get; } //无敌时间
    }
}