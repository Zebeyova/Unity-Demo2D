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
        private PlayerRunTimeData _playerData;
        private EnemyRunTimeData _enemyData;

        private void Awake()
        {
            if (isPlayer)
            {
                _playerData = GameObject.FindWithTag("Player")?.GetComponent<PlayerRunTimeData>();
                if (_playerData)
                {
                    _maxHealth = _playerData.MaxHealth;
                    _playerData.OnHurt += OnHurt;
                    OnHurt(_playerData.currentHealth, _maxHealth);
                }
            }
            else
            {
                _enemyData = GetComponentInParent<EnemyRunTimeData>();
                if (_enemyData)
                {
                    _maxHealth = _enemyData.MaxHealth;
                    _enemyData.OnHealthChanged += OnHurt;
                    OnHurt(_enemyData.currentHealth, _maxHealth);
                }
            }
        }

        private void OnDestroy()
        {
            if (_playerData) _playerData.OnHurt -= OnHurt;
            if (_enemyData) _enemyData.OnHealthChanged -= OnHurt;
        }

        private void OnHurt(float current, float max)
        {
            _maxHealth = max;
            _bufferDirty = true;
            bar.fillAmount = current / max;
            if (text) text.text = $"{current:F0} / {max:F0}";
        }

        private void Update()
        {
            if (!_bufferDirty || !bufferBar) return;
            var speed = isPlayer ? _playerData?.BufferBarSpeed ?? 2f : 2f;
            bufferBar.fillAmount = Mathf.Lerp(bufferBar.fillAmount, bar.fillAmount, Time.deltaTime * speed);
            if (Mathf.Approximately(bufferBar.fillAmount, bar.fillAmount))
                _bufferDirty = false;
        }
    }
}