using Script.Enemy;
using Script.Player;
using UnityEngine;

namespace Script
{
    public class DamageAndDefense : MonoBehaviour
    {
        private bool _allowInjured;
        private EnemyProperties _enemyProperties;
        private PlayerProperties _playerProperties;
        private GameObject _target;

        private void Awake() =>CheckComponent();

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Player"))
            {
                _allowInjured = false;
                _target = null;
            }
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

        private void CheckComponent()
        {
            _playerProperties = FindObjectOfType<PlayerProperties>();
            _enemyProperties = FindObjectOfType<EnemyProperties>();
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
        private void PlayerSkillsOperation()
        {
            if (_allowInjured)
            {
                var targetHealth = _target.GetComponent<Health>();
                if (targetHealth && targetHealth.isActiveAndEnabled)
                    targetHealth.Injured(_playerProperties.skillDamage);
            }
        }
        private void EnemyAttackOperation()
        {
            if (_allowInjured)
            {
                var targetHealth = _target.GetComponent<Health>();
                if (targetHealth && targetHealth.isActiveAndEnabled)
                    targetHealth.Injured(_enemyProperties.damage);
            }
        }
    }
}