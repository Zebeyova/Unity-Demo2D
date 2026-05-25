using Script.Enemy;
using Script.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class HealthBarEnemy : MonoBehaviour
    {
        private bool _bufferChanged;
        private Vector3 _startRotation;

        private void Awake() => CheckComponent();

        private void CheckComponent()
        {
            bufferBar = transform.parent.Find("HealthBufferBar").GetComponent<Image>();
            bar = transform.parent.Find("HealthBar").GetComponent<Image>();

            _health = transform.parent.parent.GetComponent<Health>();
            _enemyRunningData = transform.parent.parent.GetComponent<EnemyRunningData>();
            _playerRunningData = GameObject.FindWithTag("Player").GetComponent<PlayerRunningData>();
        }

        private void Start()
        {
            _health.onTakeDamage.AddListener(ChangeHealthBar);
            _startRotation = gameObject.transform.eulerAngles;
        }

        private void Update() => BufferBar();

        private void OnDestroy() => _health.onTakeDamage.RemoveListener(ChangeHealthBar);

        private void ChangeHealthBar(float damage, float currentHealth)
        {
            _bufferChanged = true;
            bar.fillAmount = currentHealth / _enemyRunningData.MaxHealth;
        }

        private void BufferBar()
        {
            transform.parent.eulerAngles = _startRotation; //保持血条不旋转
            if (!_bufferChanged) return;
            bufferBar.fillAmount = Mathf.Lerp(bufferBar.fillAmount, bar.fillAmount,
                Time.deltaTime * _playerRunningData.BufferBarSpeed);
            if (bar.fillAmount.Equals(bufferBar.fillAmount)) _bufferChanged = false;
        }

        #region 成员

        public Image bufferBar;
        public Image bar;
        private Health _health;
        private PlayerRunningData _playerRunningData;
        private EnemyRunningData _enemyRunningData;

        #endregion
    }
}