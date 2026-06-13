using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Controllers
{
    public class MainMenuController : MonoBehaviour
    {
        public void ClickNewGameButton()
        {
            SceneManager.LoadScene("Scene/GameScene");
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