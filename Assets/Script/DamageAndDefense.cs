using Script.Enemy;
using Script.Player;
using UnityEngine;

namespace Script
{
    public class DamageAndDefense : MonoBehaviour
    {
        private PlayerProperties _playerProperties;
        private EnemyProperties _enemyProperties;
        private bool _allowInjured;
        private GameObject _target;

        private void Awake()
        {
            CheckComponent();
        }

        private void CheckComponent()
        {
            if (!_playerProperties) _playerProperties = FindObjectOfType<PlayerProperties>();
            if (!_enemyProperties) _enemyProperties = FindObjectOfType<EnemyProperties>();
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                _allowInjured = true;
                _target = other.gameObject;
            }

            if (other.gameObject.CompareTag("Player"))
            {
                _allowInjured = true;
                _target = other.gameObject;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Player"))
            {
                _allowInjured = false;
                _target = null;
            }
        }

        private void PlayerAttackOperation()
        {
            if (_allowInjured)
            {
                var targetHealth = _target.GetComponent<Health>();
                if (targetHealth && targetHealth.isActiveAndEnabled)
                    targetHealth.Injured(_playerProperties.damage);
            }
        }
        private void EnemyAttackOperation()
        {
            if (_allowInjured)
            {
                var  targetHealth = _target.GetComponent<Health>();
                if (targetHealth && targetHealth.isActiveAndEnabled)
                    targetHealth.Injured(_enemyProperties.damage);
            }
        }
    }
}