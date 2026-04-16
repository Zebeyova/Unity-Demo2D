using Script.Enemy;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class HealthBarEnemy : MonoBehaviour
    {
        private bool _bufferChanged;
        private void Awake()
        {
            CheckComponent();
        }
        private void Start()
        {
            _health.onTakeDamage.AddListener(ChangeHealthBar);
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
            _otherProperties = FindObjectOfType<OtherProperties>().GetComponent<OtherProperties>();
        }
        private void ChangeHealthBar(float damage, float currentHealth)
        {
            _bufferChanged = true;
            bar.fillAmount = currentHealth / _enemyProperties.maxHealth;
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
        private Health _health;
        private EnemyProperties _enemyProperties;
        private OtherProperties _otherProperties;

        #endregion
    }
}