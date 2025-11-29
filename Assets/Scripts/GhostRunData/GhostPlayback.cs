using UnityEngine;

public class GhostPlayback : MonoBehaviour
{
    public Material ghostMaterial;
    public GameObject explodeVfx;

    GhostRunData run;
    float elapsed;

    int frameIndex;
    int skillIndex;

    Player ghostPlayer;

    bool init;

    public void Initialise(GhostRunData data)
    {
        run = data;
        init = true;
    }

    void Awake()
    {
        ghostPlayer = GetComponent<Player>();
    }

    void Start()
    {
        // disable real input
        var controller = GetComponent<PlayerControllerInputSystem>();
        if (controller != null)
            controller.enabled = false;

        if (ghostPlayer != null)
            ghostPlayer.enabled = false;

        // disable cooldowns for instant playback
        if (ghostPlayer != null)
        {
            foreach (var skill in ghostPlayer.skills)
                skill.cooldown = 0;
        }

        // apply ghost visual
        if (ghostMaterial != null)
        {
            foreach (var r in GetComponentsInChildren<Renderer>())
                r.material = ghostMaterial;
        }
    }

    void Update()
    {
        if (!init || run == null)
            return;

        elapsed += Time.deltaTime;

        UpdateMovement();
        UpdateSkills();

        if (elapsed >= run.duration)
        {
            if (explodeVfx)
                Instantiate(explodeVfx, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }

    void UpdateMovement()
    {
        if (run.frames.Count == 0) return;

        while (frameIndex < run.frames.Count - 1 &&
               run.frames[frameIndex + 1].time <= elapsed)
            frameIndex++;

        var a = run.frames[frameIndex];

        if (frameIndex < run.frames.Count - 1)
        {
            var b = run.frames[frameIndex + 1];

            float t = Mathf.InverseLerp(a.time, b.time, elapsed);
            transform.position = Vector3.Lerp(a.position, b.position, t);
            transform.rotation = Quaternion.Lerp(a.rotation, b.rotation, t);
        }
        else
        {
            transform.position = a.position;
            transform.rotation = a.rotation;
        }
    }

    void UpdateSkills()
    {
        if (ghostPlayer == null) return;

        while (skillIndex < run.skillUses.Count &&
               run.skillUses[skillIndex].time <= elapsed)
        {
            int i = run.skillUses[skillIndex].index;

            if (i >= 0 && i < ghostPlayer.skills.Count)
                ghostPlayer.skills[i].TryUse();

            skillIndex++;
        }
    }
}
