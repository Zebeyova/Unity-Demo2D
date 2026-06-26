using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Controllers
{
    public class MainMenuController : MonoBehaviour
    {
        public GameObject settingMenu;

        private void Awake()
        {
            settingMenu = GameObject.Find("SettingMenu");
        }

        private void Start()
        {
            if (settingMenu) settingMenu.SetActive(false);
        }

        public void ClickNewGameButton()
        {
            SceneManager.LoadScene("Scene/GameScene");
        }

        public void ClickSettingButton()
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
            settingMenu.SetActive(!settingMenu.activeSelf);
        }

        public void ClickQuitButton()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}