using Script.RunTimeDatas;
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

        private float _maxHealth;
        private bool _bufferDirty;
        private PlayerRunTimeData _playerRunTimeData;

        private void Awake()
        {
            _playerRunTimeData = GameObject.FindWithTag("Player")?.GetComponent<PlayerRunTimeData>();
            bar = GetComponentsInChildren<Image>()[2];
            bufferBar = GetComponentsInChildren<Image>()[1];
            if (_playerRunTimeData) _maxHealth = _playerRunTimeData.MaxHealth;
        }

        private void Start()
        {
            _playerRunTimeData.OnPlayerHurt += OnPlayerHurt;
            OnPlayerHurt(_playerRunTimeData.CurrentHealth, _maxHealth);
        }

        private void OnDestroy() => _playerRunTimeData.OnPlayerHurt -= OnPlayerHurt;


        private void Update()
        {
            if (!_bufferDirty || !bufferBar || !bar) return;
            bufferBar.fillAmount = Mathf.Lerp(bufferBar.fillAmount, bar.fillAmount, Time.deltaTime * 2f);
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