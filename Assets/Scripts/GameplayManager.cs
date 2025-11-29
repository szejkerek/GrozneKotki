using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    

    public void RestartLevel()
    {
        Bootstrap.Instance.SceneManager.ReloadCurrentScene();
    }

    public void ReturnToMenu(string menuSceneName = "MainMenu")
    {
        Time.timeScale = 1f;

        Bootstrap.Instance.SceneManager.LoadScene(menuSceneName);
    }
}
