using Script.RunTimeData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Views
{
    public class HealthBarView : MonoBehaviour
    {
        [Header("引用")] public Image bar;
        public Image bufferBar;
        public TMP_Text text;
        public bool isPlayer = true;

        private float _maxHealth;
        private bool _bufferDirty;
        private PlayerRunTimeData _playerRunTimeData;
        private EnemyRunTimeData _enemyRunTimeData;

        private void Awake()
        {
            if (isPlayer)
            {
                _playerRunTimeData = GameObject.FindWithTag("Player")?.GetComponent<PlayerRunTimeData>();
                if (!_playerRunTimeData) return;
                bar = GetComponentsInChildren<Image>()[2];
                bufferBar = GetComponentsInChildren<Image>()[1];
                _maxHealth = _playerRunTimeData.MaxHealth;
            }
            else
            {
                _enemyRunTimeData = GetComponentInParent<EnemyRunTimeData>();
                if (!_enemyRunTimeData) return;
                bar = GetComponentsInChildren<Image>()[2];
                bufferBar = GetComponentsInChildren<Image>()[1];
                _maxHealth = _enemyRunTimeData.MaxHealth;
            }
        }

        private void Start()
        {
            if (isPlayer)
            {
                _playerRunTimeData.OnPlayerHurt += OnPlayerHurt;
                OnPlayerHurt(_playerRunTimeData.currentHealth, _maxHealth);
            }
            else
            {
                _enemyRunTimeData.OnEnemyHurt += OnPlayerHurt;
                OnPlayerHurt(_enemyRunTimeData.currentHealth, _maxHealth);
            }
        }

        private void OnDestroy()
        {
            if (_playerRunTimeData) _playerRunTimeData.OnPlayerHurt -= OnPlayerHurt;
            if (_enemyRunTimeData) _enemyRunTimeData.OnEnemyHurt -= OnPlayerHurt;
        }

        private void Update()
        {
            if (!_bufferDirty || !bufferBar) return;
            var speed = isPlayer ? _playerRunTimeData.BufferBarSpeed : 2f;
            bufferBar.fillAmount = Mathf.Lerp(bufferBar.fillAmount, bar.fillAmount, Time.deltaTime * speed);
            if (Mathf.Approximately(bufferBar.fillAmount, bar.fillAmount)) _bufferDirty = false;
        }

        private void OnPlayerHurt(float current, float max)
        {
            _bufferDirty = true;
            bar.fillAmount = current / max;
            if (text) text.text = $"{current:F0} / {_maxHealth:F0}";
        }
    }
}