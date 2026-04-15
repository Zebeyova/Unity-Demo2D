using System;
using Script.Enemy;
using Script.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class HealthBar : MonoBehaviour
    {
        private bool _bufferChanged;
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
            if (!_otherProperties)
                _otherProperties = FindObjectOfType<OtherProperties>().GetComponent<OtherProperties>();
        }
        private void Update()
        {
            BufferBar();
        }
        private void ChangeHealthBar(float damage, float currentHealth)
        {
            _bufferChanged = true;
            bar.fillAmount = currentHealth / _playerProperties.maxHealth;
            text.text = $"{currentHealth} / {_playerProperties.maxHealth}";
        }
        private void BufferBar()
        {
            if (!_bufferChanged) return;
            bufferBar.fillAmount = Mathf.Lerp(bufferBar.fillAmount, bar.fillAmount,
                Time.deltaTime * _otherProperties.bufferBarSpeed);
            if (bar.fillAmount.Equals(bufferBar.fillAmount)) _bufferChanged = false;
        }

        #region 成员

        public Image bufferBar;
        public Image bar;
        public TMP_Text text;
        private Health _health;
        private PlayerProperties _playerProperties;
        private EnemyProperties _enemyProperties;
        private OtherProperties _otherProperties;

        #endregion
    }
}