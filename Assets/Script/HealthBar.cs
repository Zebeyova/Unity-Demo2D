using Script.Enemy;
using Script.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class HealthBar : MonoBehaviour
    {
        private void Awake()
        {
            CheckComponent();
        }
        private void Start()
        {
            _health.onTakeDamage.AddListener(ChangeHealthBar);
            text.text = $"{_playerProperties.maxHealth} / {_playerProperties.maxHealth}";
        }
        private void CheckComponent()
        {
            if (!bufferBar) bufferBar = transform.parent.Find("HealthBufferBar").GetComponent<Image>();
            if (!bar) bar = transform.parent.Find("HealthBar").GetComponent<Image>();
            if (!text) text = GetComponentInChildren<TMP_Text>();

            if (!_health) _health = GameObject.FindWithTag("Player").GetComponent<Health>();
            if (!_playerProperties)
                _playerProperties = FindObjectOfType<PlayerProperties>().GetComponent<PlayerProperties>();
        }
        private void ChangeHealthBar(float damage, float currentHealth)
        {
            bar.fillAmount = currentHealth / _playerProperties.maxHealth;
            text.text = $"{currentHealth} / {_playerProperties.maxHealth}";
        }

        #region 成员

        public Image bufferBar;
        public Image bar;
        public TMP_Text text;
        private Health _health;
        private PlayerProperties _playerProperties;
        private EnemyProperties _enemyProperties;

        #endregion
    }
}