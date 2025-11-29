using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private GameObject firstSelectedButton;

    private void Start()
    {
        if (firstSelectedButton != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
        
    }

    public void OnPlayClicked()
    {
        Bootstrap.Instance.SceneManager.LoadScene(gameSceneName);
    }
    

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}