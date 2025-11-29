using System.Collections;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    #region Singleton
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
    #endregion
    
    public AudioManager AudioManager;
    public SceneManager SceneManager;
    
    private IEnumerator Start()
    {
        if (Application.isEditor)
            yield break;

        yield return null;
    }
}