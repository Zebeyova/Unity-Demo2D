using Script.RunTimeDatas;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Controllers
{
    public class EscMenuController : MonoBehaviour
    {
        private GameObject _escMenu;
        private GameObject _buttonMenu;
        private GameObject _achievementMenu;
        private GameObject _gameOverMenu;
        private PlayerRunTimeData _playerRunTimeData;

        private void OnEnable()
        {
            _escMenu = GameObject.Find("EscMenu");
            _achievementMenu = _escMenu.transform.Find("AchievementMenu").gameObject;
            _gameOverMenu = GameObject.Find("GameOverMenu");
            _playerRunTimeData = GameObject.FindWithTag("Player").GetComponent<PlayerRunTimeData>();
            if (_playerRunTimeData) _playerRunTimeData.OnGameOver += OnGameOverHandler;
            Events.EventCenter.OnKilledEnemyCountChanged += OnKilledEnemyCountChangedHandler;
        }

        private void OnDisable()
        {
            _playerRunTimeData.OnGameOver -= OnGameOverHandler;
            Events.EventCenter.OnKilledEnemyCountChanged -= OnKilledEnemyCountChangedHandler;
        }

        private void Start()
        {
            if (_escMenu) _escMenu.SetActive(false);
            if (_gameOverMenu) _gameOverMenu.SetActive(false);
            OnKilledEnemyCountChangedHandler(_playerRunTimeData.killedEnemy);
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            Time.timeScale = _escMenu.activeSelf ? 1 : 0;
            _escMenu.SetActive(!_escMenu.activeSelf);
            _escMenu.transform.Find("ButtonMenu").gameObject.SetActive(true);
            _achievementMenu.gameObject.SetActive(false);
        }

        public void ClickReStartButton()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene($"Scene/{SceneManager.GetActiveScene().name}");
        }

        public void ClickMainMenuButton()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("Scene/GameScene");
        }

        public void ClickAchievementButton()
        {
            _buttonMenu = _escMenu.transform.Find("ButtonMenu").gameObject;
            _achievementMenu = _escMenu.transform.Find("AchievementMenu").gameObject;
            _buttonMenu.SetActive(!_buttonMenu.activeSelf);
            _achievementMenu.SetActive(!_achievementMenu.activeSelf);
        }

        public void ClickQuitButton()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private void OnGameOverHandler()
        {
            _gameOverMenu.SetActive(true);
        }

        private void OnKilledEnemyCountChangedHandler(int count)
        {
            if (!_achievementMenu) return;
            var achievementText = _achievementMenu.GetComponentInChildren<Transform>().gameObject
                .GetComponentInChildren<TMP_Text>();
            achievementText.text = $"已击败 : {count}";
        }
    }
}