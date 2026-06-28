using System.IO;
using Script.Models;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Controllers
{
    public class MainMenuController : MonoBehaviour
    {
        public GameObject achievementMenu;
        private string _filePath;

        private void Awake()
        {
            achievementMenu = GameObject.Find("AchievementMenu");
        }

        private void OnEnable()
        {
            if (!achievementMenu) return;
            achievementMenu.SetActive(false);
            _filePath = Application.persistentDataPath + "/killed_enemy.json";
            int killedEnemy;
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var data = JsonUtility.FromJson<KilledEnemyData>(json);
                killedEnemy = data?.killedEnemy ?? 0;
            }
            else killedEnemy = 0;
            var achievementText = achievementMenu.transform.GetComponentInChildren<TMP_Text>();
            if (achievementText) achievementText.text = $"已击败 : {killedEnemy}";
        }

        public void ClickNewGameButton()
        {
            SceneManager.LoadScene("Scene/GameScene");
        }

        public void ClickSettingButton()
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
            achievementMenu.SetActive(!achievementMenu.activeSelf);
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