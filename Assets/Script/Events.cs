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
            public ICharacterValue.Stats attackType;
        }

        public static class EventCenter
        {
            public static event Action<AttackEventArgs> OnAttackHit;

            public static void TriggerAttackHit(AttackEventArgs args)
            {
                OnAttackHit?.Invoke(args);
            }
        }
    }
}