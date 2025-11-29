using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostRunManager : MonoBehaviour
{
    public GhostPlayback ghostPrefab;

    readonly List<GhostRunData> completedRuns = new();

    void Awake()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var toSpawn = completedRuns.FindAll(r => r.sceneName == scene.name);
        SpawnAllGhosts(toSpawn);
    }

    public void AddRun(GhostRunData run)
    {
        if (run == null || run.frames.Count == 0)
            return;

        completedRuns.Add(run);
    }

    public void RemoveAllRuns()
    {
        completedRuns.Clear();
    }
    
    void SpawnAllGhosts(List<GhostRunData> toSpawn)
    {
        if (toSpawn == null)
            return;
        
        if (ghostPrefab == null)
            return;

        foreach (var run in toSpawn)
        {
            if (run.frames.Count == 0)
                continue;

            var first = run.frames[0];

            var ghostPlayback = Instantiate(
                ghostPrefab,
                first.position,
                first.rotation
            );

            ghostPlayback.Initialise(run);
        }
    }
}