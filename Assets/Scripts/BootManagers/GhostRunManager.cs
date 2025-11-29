using System.Collections.Generic;
using UnityEngine;

public class GhostRunManager : MonoBehaviour
{
    public GhostPlayback ghostPrefab;

    public List<GhostRunData> completedRuns = new();

    public static GhostRunManager Instance { get; private set; }

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

    public void RespawnAllGhosts()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (currentScene == "MainMenu")
        {
            RemoveAllRuns();
            return;
        }

        var toSpawn = completedRuns.FindAll(r => r.sceneName == currentScene);
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
        if (toSpawn == null || toSpawn.Count == 0)
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