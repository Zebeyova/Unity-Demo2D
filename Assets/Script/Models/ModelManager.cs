using UnityEngine;

namespace Script.Models
{
    public static class ModelManager
    {
        private static PlayerModelSO _playerModelSo;
        private static EnemyModelSO _enemyModelSo;

        public static PlayerModelSO PlayerModelSo
        {
            get
            {
                if (!_playerModelSo) _playerModelSo = Resources.Load<PlayerModelSO>("PlayerModel");
                if (!_playerModelSo) Debug.LogError("PlayerModelSO not loaded"); //注销这行代码大部分的"开销较大"会消失
                return _playerModelSo;
            }
        }

        public static EnemyModelSO EnemyModelSo
        {
            get
            {
                if (!_enemyModelSo) _enemyModelSo = Resources.Load<EnemyModelSO>("EnemyModel");
                if (!_enemyModelSo) Debug.LogError("EnemyModelSO not loaded");
                return _enemyModelSo;
            }
        }
    }
}