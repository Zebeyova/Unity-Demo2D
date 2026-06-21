namespace Script.Interfaces
{
    public interface IDamageable
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

        void TakeDamage(float damage); // 受到伤害
    }
}