using Script.RunTimeDatas;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Controllers
{
    public class EscMenuController : MonoBehaviour
    {
        private GameObject _escMenu;
        private GameObject _buttonMenu;
        private GameObject _settingMenu;
        public GameObject gameOverMenu;
        private PlayerRunTimeData _playerRunTimeData;

        private void OnEnable()
        {
            _escMenu = GameObject.Find("EscMenu");
            gameOverMenu = GameObject.Find("GameOverMenu");
            _playerRunTimeData = GameObject.FindWithTag("Player").GetComponent<PlayerRunTimeData>();
            if (_playerRunTimeData) _playerRunTimeData.OnGameOver += OnGameOverHandler;
        }

        private void OnDisable()
        {
            _playerRunTimeData.OnGameOver -= OnGameOverHandler;
        }

        private void Start()
        {
            if (_escMenu) _escMenu.SetActive(false);
            if (gameOverMenu) gameOverMenu.SetActive(false);
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            Time.timeScale = _escMenu.activeSelf ? 1 : 0;
            _escMenu.SetActive(!_escMenu.activeSelf);
            _escMenu.transform.Find("ButtonMenu").gameObject.SetActive(true);
            _escMenu.transform.Find("SettingMenu").gameObject.SetActive(false);
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

        public void ClickSettingButton()
        {
            _buttonMenu = _escMenu.transform.Find("ButtonMenu").gameObject;
            _settingMenu = _escMenu.transform.Find("SettingMenu").gameObject;
            _buttonMenu.SetActive(!_buttonMenu.activeSelf);
            _settingMenu.SetActive(!_settingMenu.activeSelf);
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
            Time.timeScale = 0;
            gameOverMenu.SetActive(true);
        }
    }
}