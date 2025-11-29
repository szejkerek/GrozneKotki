using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostRecorder : MonoBehaviour
{
    public float sampleInterval = 0.02f;

    GhostRunData currentRun;
    float elapsed;
    float nextSample;
    bool recording;

    void OnEnable()
    {
        StartNewRun();
    }

    void StartNewRun()
    {
        currentRun = new GhostRunData(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        elapsed = 0f;
        nextSample = 0f;
        recording = true;
    }

    void Update()
    {
        if (!recording) return;

        elapsed += Time.deltaTime;

        if (elapsed >= nextSample)
        {
            currentRun.frames.Add(new GhostFrameSample
            {
                time = elapsed,
                position = transform.position,
                rotation = transform.rotation
            });

            nextSample += sampleInterval;
        }
    }

    public void RecordSkillUse(int index)
    {
        if (!recording) return;

        currentRun.skillUses.Add(new GhostSkillUseSample
        {
            time = elapsed,
            index = index
        });
    }

    public void StopAndStore()
    {
        if (!recording) return;

        recording = false;
        currentRun.duration = elapsed;

        Bootstrap.Instance.GhostRunManager.AddRun(currentRun);
    }

    void OnDestroy()
    {
        if (recording)
            StopAndStore();
    }
}