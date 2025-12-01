using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private GameObject firstSelectedButton;
    [SerializeField] private GameObject tutorialObject;
    [SerializeField] private GameObject tutorialImage;
    [SerializeField] private Image filledTutorial;

    private bool isWaitingForInput;
    private Vector3 tutorialBaseScale;

    private void Start()
    {
        if (firstSelectedButton != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }

        // if (tutorialImage != null)
        // {
        //     tutorialBaseScale = tutorialImage.transform.localScale;
        // }
        //
        // if (tutorialObject != null)
        // {
        //     tutorialObject.SetActive(false);
        // }
        //
        // if (filledTutorial != null)
        // {
        //     filledTutorial.fillAmount = 0f;
        // }
    }

    public void OnPlayClicked()
    {
        if (isWaitingForInput)
            return;

        Bootstrap.Instance.SceneManager.LoadScene(gameSceneName);

        //StartCoroutine(ShowTutorialAndWaitForKey());
    }

    private IEnumerator ShowTutorialAndWaitForKey()
    {
        isWaitingForInput = true;

        if (tutorialObject != null)
        {
            tutorialObject.SetActive(true);
        }

        if (tutorialImage != null)
        {
            tutorialImage.transform.localScale = tutorialBaseScale;
        }

        if (filledTutorial != null)
        {
            filledTutorial.fillAmount = 1f; // instant full tutorial indicator
        }

        // wait until any key is pressed with the Input System
        while (true)
        {
            bool keyboardPressed =
                Keyboard.current != null &&
                Keyboard.current.anyKey.wasPressedThisFrame;

            bool mousePressed =
                Mouse.current != null &&
                (Mouse.current.leftButton.wasPressedThisFrame ||
                 Mouse.current.rightButton.wasPressedThisFrame ||
                 Mouse.current.middleButton.wasPressedThisFrame);

            if (keyboardPressed || mousePressed)
            {
                break;
            }

            yield return null;
        }

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
