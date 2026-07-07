using System.IO;
using Script.RunTimeDatas;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Script.HotFix
{
    public class ILRuntimeManager : MonoBehaviour
    {
        private AppDomain _appDomain;

        private void Awake()
        {
            _appDomain = new AppDomain(); //创建ILRuntime解释器域
            _appDomain.RegisterCrossBindingAdaptor(new DamageSystemAdaptor()); //实例化跨域继承适配器

            var dllPath = Application.streamingAssetsPath + "/net9.0/HotFix.dll";
            if (!File.Exists(dllPath)) return;
            var dllBytes = File.ReadAllBytes(dllPath);
            using var memoryStream = new MemoryStream(dllBytes);
            _appDomain.LoadAssembly(memoryStream);
            _appDomain.Invoke("HotFix.Entry", "ILStart", null, null);

            var damageCalc = _appDomain.Instantiate<DamageSystem>("HotFix.System.DamageCalculate");
            damageCalc?.Initialize();
        }

        private void OnDestroy()
        {
            DamageSystem.Instance?.CleanUp();
        }
    }
}