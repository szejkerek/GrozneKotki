using UnityEngine;

public static class AutoBootstrap
{
    private const string BootstrapResourcePath = "Bootstrap/Bootstrap";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Spawn()
    {
        if (Object.FindFirstObjectByType<Bootstrap>() != null) return;

        var prefab = Resources.Load<GameObject>(BootstrapResourcePath);
        if (prefab == null)
        {
            Debug.LogError($"[AutoBootstrap] Missing prefab at Resources/{BootstrapResourcePath}. " +
                           "Create it or change the path in AutoBootstrap.");
            return;
        }

        if (prefab.GetComponent<Bootstrap>() == null)
        {
            Debug.LogError("[AutoBootstrap] Prefab at path does not contain Bootstrap component.");
            return;
        }
        
        var go = Object.Instantiate(prefab); 
        go.name = prefab.name; 
        Object.DontDestroyOnLoad(go);
    }
}