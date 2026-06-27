using System;
using UnityEngine;

namespace Script.Models
{
    [System.Serializable]
    public class LevelModel : MonoBehaviour
    {
        [Tooltip("与此关卡关联的场景名称")] public string sceneName = "";
        [Tooltip("关卡描述")] public string description = "";

        private void Awake()
        {
            if (sceneName == "") sceneName = gameObject.name;
        }
    }
}