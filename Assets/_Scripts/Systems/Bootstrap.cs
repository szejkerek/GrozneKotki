using System.Collections;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class Bootstrap : MonoBehaviour
{
    public static Bootstrap Instance;

    void Awake()
    {
        Debug.Log("ProjectContexts: " + FindObjectsOfType<ProjectContext>().Length);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
        DOTween.defaultAutoPlay = AutoPlay.All;
        DOTween.defaultAutoKill = true;
    }
    
    public AudioManager AudioManager;
    public SceneController SceneManager;
    public GhostRunManager GhostRunManager;
    
    
    private IEnumerator Start()
    {
        if (Application.isEditor)
            yield break;

        yield return null;
    }
}