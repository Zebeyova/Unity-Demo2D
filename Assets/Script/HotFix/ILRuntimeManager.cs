using System.IO;
using ILRuntime.Runtime.Enviorment;
using UnityEngine;

namespace Script.HotFix
{
    public class ILRuntimeManager : MonoBehaviour
    {
        private AppDomain _appDomain;

        private void Awake()
        {
            _appDomain = new AppDomain();
            var dllPath = Application.streamingAssetsPath + "/net9.0/HotFix.dll";
            if (!File.Exists(dllPath)) return;
            var dllBytes = File.ReadAllBytes(dllPath);
            using var memoryStream = new MemoryStream(dllBytes);
            _appDomain.LoadAssembly(memoryStream);
            _appDomain.Invoke("HotFix.Entry", "ILStart", null, null);
        }
    }
}