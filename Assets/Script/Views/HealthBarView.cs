using Script.RunTimeDatas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Views
{
    public class HealthBarView : MonoBehaviour
    {
        [Header("血条引用")] public Image healthBar;
        public Image bufferBar;
        public TMP_Text text;
        [Header("经验条引用")] public Image expBar;
        private float _maxHealth;
        private bool _bufferDirty;
        private PlayerRunTimeData _playerRunTimeData;

        private void Awake()
        {
            _playerRunTimeData = GameObject.FindWithTag("Player")?.GetComponent<PlayerRunTimeData>();
            healthBar = GetComponentsInChildren<Image>()[2];
            bufferBar = GetComponentsInChildren<Image>()[1];
            if (_playerRunTimeData) _maxHealth = _playerRunTimeData.MaxHealth;
        }

        private void Start()
        {
            _playerRunTimeData.OnPlayerHurt += OnPlayerHurt;
            _playerRunTimeData.OnPlayerExperienceChanged += OnPlayerExpChanged;
            OnPlayerHurt(_playerRunTimeData.CurrentHealth, _maxHealth);
        }

        private void OnDestroy()
        {
            _playerRunTimeData.OnPlayerHurt -= OnPlayerHurt;
        }


        private void Update()
        {
            if (!_bufferDirty || !bufferBar || !healthBar) return;
            bufferBar.fillAmount = Mathf.Lerp(bufferBar.fillAmount, healthBar.fillAmount, Time.deltaTime * 2f);
            if (Mathf.Approximately(bufferBar.fillAmount, healthBar.fillAmount)) _bufferDirty = false;
        }

        private void OnPlayerHurt(float current, float max)
        {
            _bufferDirty = true;
            healthBar.fillAmount = current / max;
            if (text) text.text = $"{current:F0} / {_maxHealth:F0}";
        }

        private void OnPlayerExpChanged(float currentExp, float maxExp)
        {
            if (!expBar) return;
            expBar.fillAmount = currentExp / maxExp;
        }
    }
}