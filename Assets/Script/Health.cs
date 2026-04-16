using System.Collections;
using Script.Enemy;
using Script.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Script
{
    public class Health : MonoBehaviour
    {
        private void Awake()
        {
            CheckComponent();
        }

        private void Start()
        {
            currentHealth = _playerProperties.maxHealth;
        }

        public void Injured(float damage)
        {
            if (_invincible || damage < 0) return;
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, _playerProperties.maxHealth); //确保生命不会出现负数
            onTakeDamage?.Invoke(damage, currentHealth);
            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                if (_invincibilityCoroutine != null) StopCoroutine(_invincibilityCoroutine);
                _invincibilityCoroutine = StartCoroutine(EnableInvincibility());
            }
        }

        private void Die()
        {
            onDeath?.Invoke();
        }

        private void CheckComponent()
        {
            if (!_playerProperties) _playerProperties = FindObjectOfType<PlayerProperties>();
            if (!_enemyProperties) _enemyProperties = FindObjectOfType<EnemyProperties>();
        }

        #region 属性

        public float currentHealth;
        private bool _invincible;
        private Coroutine _invincibilityCoroutine;

        private IEnumerator EnableInvincibility() //无敌计时协程
        {
            _invincible = true;
            yield return new WaitForSeconds(_playerProperties.invincibleTime);
            _invincible = false;
        }

        #endregion

        #region 成员

        private PlayerProperties _playerProperties;
        private EnemyProperties _enemyProperties;
        public UnityEvent<float, float> onTakeDamage; //受伤事件广播
        public UnityEvent onDeath; //死亡事件广播

        #endregion
    }
}