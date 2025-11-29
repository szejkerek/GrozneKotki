using System.Collections;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    public static Bootstrap Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public AudioManager AudioManager;
    public SceneManager SceneManager;
    public GhostRunManager GhostRunManager;
    
    
    private IEnumerator Start()
    {
        if (Application.isEditor)
            yield break;

        yield return null;
    }
}