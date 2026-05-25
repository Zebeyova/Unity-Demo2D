using System.Collections;
using Script.Enemy;
using Script.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Script
{
    public class Health : MonoBehaviour
    {
        private void Awake() => CheckComponent();

        private void CheckComponent()
        {
            _enemyRunningData = FindObjectOfType<EnemyRunningData>();
            _playerRunningData = GetComponent<PlayerRunningData>();
        }

        private void Start()
        {
            currentHealth = transform.CompareTag("Player")
                ? _playerRunningData.currentHealth
                : _enemyRunningData.MaxHealth;
        }

        public void Injured(float damage)
        {
            if (_invincible || damage < 0) return;
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0,
                transform.CompareTag("Player")
                    ? _playerRunningData.MaxHealth
                    : _enemyRunningData.MaxHealth); //确保生命不会出现负数
            onTakeDamage?.Invoke(damage, currentHealth);
            if (currentHealth <= 0)
            {
                onDeath?.Invoke();
            }
            else
            {
                if (_invincibilityCoroutine != null) StopCoroutine(_invincibilityCoroutine);
                _invincibilityCoroutine = StartCoroutine(EnableInvincibility());
            }
        }

        private IEnumerator EnableInvincibility() //无敌计时协程
        {
            _invincible = true;
            yield return new WaitForSeconds(_playerRunningData.InvincibleTime);
            _invincible = false;
        }

        #region 属性

//TODO: 需要处理,有很大问题
        public float currentHealth;
        private bool _invincible;
        private Coroutine _invincibilityCoroutine;

        #endregion

        #region 成员

        private EnemyRunningData _enemyRunningData;
        private PlayerRunningData _playerRunningData;
        public UnityEvent<float, float> onTakeDamage; //受伤事件广播
        public UnityEvent onDeath; //死亡事件广播

        #endregion
    }
}