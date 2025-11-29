using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{

    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[SceneController] LoadScene called with null or empty scene name");
            return;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int buildIndex)
    {
        if (buildIndex < 0 || buildIndex >= UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"[SceneController] Invalid build index {buildIndex}");
            return;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(buildIndex);
    }

    public void ReloadCurrentScene()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene.buildIndex);
    }

    public void LoadNextScene()
    {
        var currentIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        var nextIndex = currentIndex + 1;

        if (nextIndex >= UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("[SceneController] No next scene in build settings");
            return;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(nextIndex);
    }

    public int GetCurrentSceneIndex()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
    }

    public string GetCurrentSceneName()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }
}