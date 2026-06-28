using System;
using Script.Interfaces;
using UnityEngine;

namespace Script
{
    public static class Events
    {
        public class AttackEventArgs
        {
            public GameObject Attacker;
            public GameObject Target;
            public IDamageable.Stats AttackType;
        }

        public class DamageEventArgs
        {
            public GameObject Target;
            public float Damage;
        }

        public class ExperienceEventArgs
        {
            public float Experience;
        }

        public class EnemyDeathEventArgs
        {
            public Vector3 Position;
            public float Experience;
        }

        public static class EventCenter
        {
            public static event Action<AttackEventArgs> OnAttackHit;
            public static event Action<DamageEventArgs> OnDamageResolved;
            public static event Action<ExperienceEventArgs> OnExperienceCollected;
            public static event Action<EnemyDeathEventArgs> OnEnemyDeath;

            public static void TriggerAttackHit(AttackEventArgs args)
            {
                OnAttackHit?.Invoke(args);
            }

            public static void TriggerDamageResolved(DamageEventArgs args)
            {
                OnDamageResolved?.Invoke(args);
            }

            public static void TriggerExperienceCollected(ExperienceEventArgs args)
            {
                OnExperienceCollected?.Invoke(args);
            }

            public static void TriggerEnemyDefeated(EnemyDeathEventArgs args)
            {
                OnEnemyDeath?.Invoke(args);
            }
        }
    }
}