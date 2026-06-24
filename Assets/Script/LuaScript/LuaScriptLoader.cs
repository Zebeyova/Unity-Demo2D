using System.IO;
using UnityEngine;
using XLua;

namespace Script.LuaScript
{
    public class LuaScriptLoader : MonoBehaviour
    {
        private LuaEnv _luaEnv;

        private void Start()
        {
            _luaEnv = new LuaEnv();
            _luaEnv.AddLoader(CustomLoader);
            _luaEnv.DoString("require 'Main'");
        }

        private void OnDestroy()
        {
            if (_luaEnv == null) return;
            _luaEnv.Tick();
            _luaEnv.Dispose();
            _luaEnv = null;
        }

        private byte[] CustomLoader(ref string fileName)
        {
            var filePath = Application.dataPath + "/Script/LuaScript/" + fileName + ".lua";
            if (File.Exists(filePath))
            {
                return File.ReadAllBytes(filePath);
            }

            Debug.LogError("CustomLoader not found Lua script file in: " + filePath);
            return null;
        }
    }
}