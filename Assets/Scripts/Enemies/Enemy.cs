using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamagable
{
    private Transform core;
    private NavMeshPath path;
    private List<Vector3> points;
    private int index;
    private Vector3 targetPoint;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float minDelta = 0.05f;

    [Header("Time rewind")]
    [SerializeField] private int maxStoredStates = 700;      // ile klatek historii
    [SerializeField] private float rewindPlaybackDuration = 1f; // ile sekund ma trwać same cofanie

    private Rigidbody rb;

    private bool rewinding;

    // parametry aktualnego rewinda
    private int statesToRewindLeft;
    private float statesPerFrame;
    private float statesStepAccumulator;

    private struct EnemyState
    {
        public Vector3 position;
        public Quaternion rotation;
        public int pathIndex;
        public Vector3 targetPoint;

        public EnemyState(Vector3 pos, Quaternion rot, int idx, Vector3 tgt)
        {
            position = pos;
            rotation = rot;
            pathIndex = idx;
            targetPoint = tgt;
        }
    }

    private readonly List<EnemyState> history = new List<EnemyState>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        core = GameObject.FindGameObjectWithTag("Core").transform;

        path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, core.position, NavMesh.AllAreas, path);

        points = new List<Vector3>(path.corners);

        if (points.Count > 0)
        {
            index = 0;
            targetPoint = points[index];
        }
        else
        {
            targetPoint = core.position;
        }
    }

    private void Update()
    {
        if (rewinding)
            return;

        if (points == null || points.Count == 0)
            return;

        Vector3 lookDir = targetPoint - transform.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = targetRot;
        }
    }

    private void FixedUpdate()
    {
        if (rewinding)
        {
            RewindStep();
        }
        else
        {
            RecordState();
            MoveAlongPath();
        }
    }

    private void MoveAlongPath()
    {
        if (points == null || points.Count == 0)
            return;

        float distance = Vector3.Distance(transform.position, targetPoint);

        if (distance <= minDelta)
        {
            if (index < points.Count - 1)
            {
                index++;
                targetPoint = points[index];
            }
            else
            {
                return;
            }
        }

        Vector3 dir = (targetPoint - transform.position).normalized;
        Vector3 step = dir * speed * Time.fixedDeltaTime;

        if (rb != null && rb.isKinematic)
        {
            rb.MovePosition(rb.position + step);
        }
        else
        {
            transform.position += step;
        }
    }

    private void RecordState()
    {
        if (history.Count >= maxStoredStates)
        {
            history.RemoveAt(0);
        }

        history.Add(new EnemyState(transform.position, transform.rotation, index, targetPoint));
    }

    private void RewindStep()
    {
        if (statesToRewindLeft <= 0 || history.Count == 0)
        {
            rewinding = false;
            return;
        }

        // ile stanów powinniśmy zużyć w tej klatce
        statesStepAccumulator += statesPerFrame;
        int stepsThisFrame = Mathf.FloorToInt(statesStepAccumulator);
        if (stepsThisFrame <= 0)
            stepsThisFrame = 1;

        stepsThisFrame = Mathf.Min(stepsThisFrame, statesToRewindLeft, history.Count);

        for (int i = 0; i < stepsThisFrame; i++)
        {
            int lastIndex = history.Count - 1;
            EnemyState state = history[lastIndex];

            // przywrócenie pozycji i rotacji
            if (rb != null && rb.isKinematic)
            {
                rb.MovePosition(state.position);
                rb.MoveRotation(state.rotation);
            }
            else
            {
                transform.position = state.position;
                transform.rotation = state.rotation;
            }

            // przywrócenie info o ścieżce
            index = state.pathIndex;
            targetPoint = state.targetPoint;

            history.RemoveAt(lastIndex);
            statesToRewindLeft--;
        }

        statesStepAccumulator -= stepsThisFrame;

        if (statesToRewindLeft <= 0 || history.Count == 0)
        {
            rewinding = false;
        }
    }

    // rewindAmountSeconds – ile czasu historii chcesz cofnąć (np. 3f)
    public void ReverseTime(float rewindAmountSeconds)
    {
        if (rewinding)
            return;

        if (history.Count <= 1 || rewindAmountSeconds <= 0f)
            return;

        // ile stanów mamy dostępnych
        int availableStates = history.Count;
        int desiredStates = Mathf.RoundToInt(rewindAmountSeconds / Time.fixedDeltaTime);
        desiredStates = Mathf.Clamp(desiredStates, 1, availableStates);

        statesToRewindLeft = desiredStates;

        float framesNeeded = rewindPlaybackDuration / Time.fixedDeltaTime;
        if (framesNeeded <= 0f)
            framesNeeded = 1f;

        statesPerFrame = statesToRewindLeft / framesNeeded;
        if (statesPerFrame < 1f)
            statesPerFrame = 1f;

        statesStepAccumulator = 0f;
        rewinding = true;
    }

    public void TakeDamage(float damage)
    {
        // tu możesz np. triggerować ReverseTime:
        // ReverseTime(3f);
    }
}
