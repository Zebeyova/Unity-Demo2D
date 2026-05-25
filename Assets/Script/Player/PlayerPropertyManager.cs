using Resources;
using UnityEngine;

namespace Script.Player
{
    public static class PlayerPropertyManager
    {
        private static PlayerProperty _playerProperty;

        public static PlayerProperty PlayerProperty
        {
            get
            {
                if (!_playerProperty) _playerProperty = UnityEngine.Resources.Load<PlayerProperty>("PlayerProperty");
                if (!_playerProperty) Debug.LogError("PlayerProperty not loaded");
                return _playerProperty;
            }
        }
    }
}