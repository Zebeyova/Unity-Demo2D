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
        private void Awake() => CheckComponent();

        private void Start()
        {
            _health.onTakeDamage.AddListener(ChangeHealthBar);
            text.text = $"{PlayerPropertyManager.PlayerProperty.maxHealth} / {PlayerPropertyManager.PlayerProperty.maxHealth}";
        }
        private void Update()
        {
            BufferBar();
            slideCoolDownBar.fillAmount = _playerController.GetSlideTimer() / PlayerPropertyManager.PlayerProperty.slideCool; //滑铲冷却条
        }
        private void OnDestroy() => _health.onTakeDamage.RemoveListener(ChangeHealthBar);

        private void CheckComponent()
        {
            bufferBar = transform.parent.Find("HealthBufferBar").GetComponent<Image>();
            bar = transform.parent.Find("HealthBar").GetComponent<Image>();
            slideCoolDownBar = transform.parent.Find("SlideCoolDownBar").GetComponent<Image>();
            text = GetComponentInChildren<TMP_Text>();

            _health = GameObject.FindWithTag("Player").GetComponent<Health>();
            _playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        }
        private void ChangeHealthBar(float damage, float currentHealth) //血条
        {
            _bufferChanged = true;
            bar.fillAmount = currentHealth / PlayerPropertyManager.PlayerProperty.maxHealth;
            text.text = $"{currentHealth} / {PlayerPropertyManager.PlayerProperty.maxHealth}";
        }
        private void BufferBar() //血量缓冲条
        {
            if (!_bufferChanged) return;
            bufferBar.fillAmount = Mathf.Lerp(bufferBar.fillAmount, bar.fillAmount,
                Time.deltaTime * PlayerPropertyManager.PlayerProperty.bufferBarSpeed);
            if (bar.fillAmount.Equals(bufferBar.fillAmount)) _bufferChanged = false;
        }

        #region 成员

        public Image bufferBar;
        public Image bar;
        public Image slideCoolDownBar;
        public TMP_Text text;
        private Health _health;
        private PlayerController _playerController;
        private EnemyProperties _enemyProperties;

        #endregion
    }
}