using Script.RunTimeDatas;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Views
{
    public class EnemyHealthBarView : MonoBehaviour
    {
        [Header("引用")] public Image bar;
        public Image bufferBar;
        private EnemyRunTimeData _enemyRunTimeData;
        private GameObject _enemy;
        private float _maxHealth;
        private bool _bufferDirty;

        private void Awake()
        {
            _enemyRunTimeData = transform.parent.GetComponentInChildren<EnemyRunTimeData>();
            if (!_enemyRunTimeData) return;
            bar = GetComponentsInChildren<Image>()[2];
            bufferBar = GetComponentsInChildren<Image>()[1];
            _maxHealth = _enemyRunTimeData.MaxHealth;
        }

        private void Start()
        {
            _enemyRunTimeData.OnEnemyHurt += OnEnemyHurt;
            OnEnemyHurt(_enemyRunTimeData.CurrentHealth, _maxHealth);
            _enemy = _enemyRunTimeData.gameObject;
        }

        private void OnDestroy()
        {
            if (_enemyRunTimeData) _enemyRunTimeData.OnEnemyHurt -= OnEnemyHurt;
        }

        private void Update()
        {
            if (!_bufferDirty || !bufferBar) return;
            bufferBar.fillAmount = Mathf.Lerp(bufferBar.fillAmount, bar.fillAmount, Time.deltaTime * 2f);
            if (Mathf.Approximately(bufferBar.fillAmount, bar.fillAmount)) _bufferDirty = false;
        }

        private void LateUpdate() //更新血条位置
        {
            var enemyPosition = _enemy.transform.position;
            transform.position = new Vector3(enemyPosition.x, enemyPosition.y + 1.5f, enemyPosition.z);
        }

        private void OnEnemyHurt(float current, float max)
        {
            _bufferDirty = true;
            bar.fillAmount = current / max;
        }
    }
}