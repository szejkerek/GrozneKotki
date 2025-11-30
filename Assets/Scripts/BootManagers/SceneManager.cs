using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneController : MonoBehaviour
{
    [Header("Transition")]
    [SerializeField] private Image transitionImage;
    [SerializeField] private float transitionDuration = 0.4f;
    [SerializeField] private Ease fadeEase = Ease.InOutQuad;
    [SerializeField] private Color transitionColor = Color.white;

    // flaga mówi czy po załadowaniu sceny mamy zrobić fade z bieli
    private bool fadeOnNextSceneLoad = true;

    private Tween currentTween;

    void Awake()
    {
        if (transitionImage != null)
        {
            transitionImage.gameObject.SetActive(false);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!fadeOnNextSceneLoad || transitionImage == null)
            return;

        fadeOnNextSceneLoad = false;

        PlayFade(1f, 0f, null);    // z pełnej bieli do przezroczystości
    }

    /// <summary>
    /// Uniwersalny tween dla przejść
    /// </summary>
    private void PlayFade(float fromAlpha, float toAlpha, TweenCallback onCompleted)
    {
        if (transitionImage == null)
            return;

        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        transitionImage.gameObject.SetActive(true);

        var c = transitionColor;
        c.a = fromAlpha;
        transitionImage.color = c;

        currentTween = transitionImage
            .DOFade(toAlpha, transitionDuration)
            .SetEase(fadeEase)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                currentTween = null;

                if (toAlpha <= 0f)
                {
                    transitionImage.gameObject.SetActive(false);
                }

                onCompleted?.Invoke();
            });
    }

    public void ReloadCurrentScene()
    {
        if (transitionImage == null)
        {
            Debug.LogWarning("[SceneController] No transition image set, reloading instantly");
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);
            return;
        }

        // mówimy że przy kolejnym wczytaniu sceny ma być fade z bieli
        fadeOnNextSceneLoad = true;

        // z przezroczystej bieli do pełnej bieli, potem reload
        PlayFade(0f, 1f, () =>
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);
        });
    }

    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[SceneController] LoadScene called with null or empty scene name");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int buildIndex)
    {
        if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"[SceneController] Invalid build index {buildIndex}");
            return;
        }

        SceneManager.LoadScene(buildIndex);
    }

    
}
