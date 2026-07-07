using System;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using Script.Interfaces;
using Script.RunTimeDatas;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Script.HotFix
{
    public class DamageSystemAdaptor : CrossBindingAdaptor
    {
        public override Type BaseCLRType => typeof(DamageSystem);
        public override Type AdaptorType => typeof(Adaptor);

        public override object CreateCLRInstance(AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);
        }

        private static readonly CrossBindingMethodInfo HotFixOnEnable =
            new CrossBindingMethodInfo("OnEnable");

        private static readonly CrossBindingMethodInfo HotFixOnDisable =
            new CrossBindingMethodInfo("OnDisable");

        private static readonly CrossBindingFunctionInfo<DamageSystem.DamageCache, IDamageable.Stats, float>
            HotFixReadAttackPower =
                new CrossBindingFunctionInfo<DamageSystem.DamageCache, IDamageable.Stats, float>(
                    "ReadAttackPower");

        private static readonly CrossBindingFunctionInfo<DamageSystem.DamageCache, float>
            HotFixReadDefensePower =
                new CrossBindingFunctionInfo<DamageSystem.DamageCache, float>("ReadDefensePower");

        private static readonly CrossBindingFunctionInfo<float, float, float>
            HotFixCalculateFinalDamage =
                new CrossBindingFunctionInfo<float, float, float>("CalculateFinalDamage");

        private class Adaptor : DamageSystem, CrossBindingAdaptorType
        {
            private ILTypeInstance _instance;
            private AppDomain _appDomain;

            public Adaptor() { }

            public Adaptor(AppDomain appDomain, ILTypeInstance instance)
            {
                _appDomain = appDomain;
                _instance = instance;
            }

            public ILTypeInstance ILInstance
            {
                get => _instance;
                set => _instance = value;
            }

            protected override void OnEnable()
            {
                Debug.Log("[Adaptor] HotFixOnEnable invoked successfully");
                if (_instance != null)
                {
                    HotFixOnEnable.Invoke(_instance);
                }
                else
                {
                    base.OnEnable();
                }
            }

            protected override void OnDisable()
            {
                Debug.Log("[Adaptor] OnDisable called");
                if (_instance != null)
                {
                    HotFixOnDisable.Invoke(_instance);
                }
                else
                {
                    base.OnDisable();
                }
            }


            protected override float ReadAttackPower(DamageCache attacker, IDamageable.Stats damageType)
            {
                return _instance != null
                    ? HotFixReadAttackPower.Invoke(_instance, attacker, damageType)
                    : base.ReadAttackPower(attacker, damageType);
            }

            protected override float ReadDefensePower(DamageCache defender)
            {
                Debug.Log("[Adaptor] ReadDefensePower called");
                return _instance != null
                    ? HotFixReadDefensePower.Invoke(_instance, defender)
                    : base.ReadDefensePower(defender);
            }

            protected override float CalculateFinalDamage(float attack, float defense)
            {
                if (_instance != null && HotFixCalculateFinalDamage.CheckShouldInvokeBase(_instance))
                    return HotFixCalculateFinalDamage.Invoke(_instance, attack, defense);
                return base.CalculateFinalDamage(attack, defense);
            }
        }
    }
}