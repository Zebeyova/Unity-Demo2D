using System.Linq;
using Script.Models;
using UnityEditor;
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
            if (sceneName != "" && EditorBuildSettings.scenes.Any(scene => scene.path.Contains(sceneName)))
            {
                SceneManager.LoadScene($"Scene/{sceneName}");
            }
            else
            {
                Debug.LogWarning(
                    $"{currentSelectedButton.name} does not have a valid scene name or the scene is not added to the build settings.");
            }
        }
    }
}