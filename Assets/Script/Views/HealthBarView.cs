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
            bufferBar = GetComponentsInChildren<Image>()[3];
            healthBar = GetComponentsInChildren<Image>()[4];
            if (_playerRunTimeData) _maxHealth = _playerRunTimeData.MaxHealth;
        }

        private void Start()
        {
            if (!_playerRunTimeData) return;
            _playerRunTimeData.OnPlayerHurt += OnPlayerHealthBarChanged;
            _playerRunTimeData.OnPlayerExperienceChanged += OnPlayerExpBarChanged;
            OnPlayerHealthBarChanged(_playerRunTimeData.CurrentHealth, _maxHealth);
            OnPlayerExpBarChanged(_playerRunTimeData.Experience, _playerRunTimeData.ExperienceToNextLevel);
        }

        private void OnDestroy()
        {
            if (!_playerRunTimeData) return;
            _playerRunTimeData.OnPlayerHurt -= OnPlayerHealthBarChanged;
            _playerRunTimeData.OnPlayerExperienceChanged -= OnPlayerExpBarChanged;
        }


        private void Update()
        {
            if (!_bufferDirty || !bufferBar || !healthBar) return;
            bufferBar.fillAmount = Mathf.Lerp(bufferBar.fillAmount, healthBar.fillAmount, Time.deltaTime * 2f);
            if (Mathf.Approximately(bufferBar.fillAmount, healthBar.fillAmount)) _bufferDirty = false;
        }

        private void OnPlayerHealthBarChanged(float current, float max)
        {
            _bufferDirty = true;
            var temp = current / max;
            healthBar.fillAmount = temp;
            if (text) text.text = $"{temp * 100f} %";
        }

        private void OnPlayerExpBarChanged(float currentExp, float maxExp)
        {
            if (!expBar) return;
            expBar.fillAmount = maxExp <= 0f ? 0f : Mathf.Clamp01(currentExp / maxExp);
        }
    }
}