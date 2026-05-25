using UnityEngine;

namespace Resources
{
    public static class PropertyManager
    {
        private static PlayerProperty _playerProperty;
        private static EnemyProperty _enemyProperty;

        public static PlayerProperty PlayerProperty
        {
            get
            {
                if (!_playerProperty) _playerProperty = UnityEngine.Resources.Load<PlayerProperty>("PlayerProperty");
                if (!_playerProperty) Debug.LogError("PlayerProperty not loaded"); //注销这行代码大部分的"开销较大"会消失
                return _playerProperty;
            }
        }

        public static EnemyProperty EnemyProperty
        {
            get
            {
                if (!_enemyProperty) _enemyProperty = UnityEngine.Resources.Load<EnemyProperty>("EnemyProperty");
                if (!_enemyProperty) Debug.LogError("EnemyProperty not loaded");
                return _enemyProperty;
            }
        }
    }
}