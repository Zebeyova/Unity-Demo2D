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
        private void Awake()
        {
            CheckComponent();
        }
        private void Start()
        {
            _health.onTakeDamage.AddListener(ChangeHealthBar);
            _startRotation = gameObject.transform.eulerAngles;
        }
        private void Update()
        {
            BufferBar();
        }
        private void CheckComponent()
        {
            bufferBar = transform.parent.Find("HealthBufferBar").GetComponent<Image>();
            bar = transform.parent.Find("HealthBar").GetComponent<Image>();

            _health = transform.parent.parent.GetComponent<Health>();
            _enemyProperties = FindObjectOfType<EnemyProperties>().GetComponent<EnemyProperties>();
            _playerProperties = FindObjectOfType<PlayerProperties>().GetComponent<PlayerProperties>();
        }
        private void ChangeHealthBar(float damage, float currentHealth)
        {
            _bufferChanged = true;
            bar.fillAmount = currentHealth / _enemyProperties.maxHealth;
        }
        private void BufferBar()
        {
            transform.parent.eulerAngles = _startRotation; //保持血条不旋转
            if (!_bufferChanged) return;
            bufferBar.fillAmount = Mathf.Lerp(bufferBar.fillAmount, bar.fillAmount,
                Time.deltaTime * _playerProperties.bufferBarSpeed);
            if (bar.fillAmount.Equals(bufferBar.fillAmount)) _bufferChanged = false;
        }

        #region 成员

        public Image bufferBar;
        public Image bar;
        private Health _health;
        private EnemyProperties _enemyProperties;
        private PlayerProperties _playerProperties;

        #endregion
    }
}