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

        void ApplyDamage(float damage);
    }
}