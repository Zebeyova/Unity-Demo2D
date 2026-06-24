using System;
using Script.Interfaces;
using UnityEngine;

namespace Script
{
    public static class Events
    {
        public class AttackEventArgs
        {
            public GameObject attacker;
            public GameObject target;
            public IDamageable.Stats attackType;
        }

        public class DamageEventArgs
        {
            public GameObject target;
            public float damage;
        }

        public class ExperienceEventArgs
        {
            public float experience;
        }

        public class EnemyDeathEventArgs
        {
            public Vector3 position;
            public float experience;
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