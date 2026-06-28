using Script.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Script.Controllers
{
    public class SelectSceneController : MonoBehaviour
    {
        public void OnClickReturnButton()
        {
            SceneManager.LoadScene("Scene/MainScene");
        }

        public void OnClickLevelButton()
        {
            var currentSelectedButton = EventSystem.current.currentSelectedGameObject;
            var sceneName = currentSelectedButton.GetComponent<LevelModel>().sceneName;
            if (!string.IsNullOrEmpty(sceneName) && Application.CanStreamedLevelBeLoaded(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogWarning(
                    $"{currentSelectedButton.name} does not have a valid scene name or the scene is not added to the build settings.");
            }
        }
    }
}