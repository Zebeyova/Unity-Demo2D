using UnityEngine;

namespace Script.Models
{
    public static class ModelManager
    {
        private static PlayerModelSObject _playerModelSObject;
        private static EnemyModelSObject _enemyModelSObject;

        public static PlayerModelSObject PlayerModelSObject
        {
            get
            {
                if (!_playerModelSObject) _playerModelSObject = Resources.Load<PlayerModelSObject>("PlayerModel");
                if (!_playerModelSObject) Debug.LogError("PlayerModelSObject not loaded");
                return _playerModelSObject;
            }
        }

        public static EnemyModelSObject EnemyModelSObject
        {
            get
            {
                if (!_enemyModelSObject) _enemyModelSObject = Resources.Load<EnemyModelSObject>("EnemyModel");
                if (!_enemyModelSObject) Debug.LogError("EnemyModelSObject not loaded");
                return _enemyModelSObject;
            }
        }
    }
}