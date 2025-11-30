using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamagable
{
    public static event System.Action OnEnemyKilled;

    private static readonly int BackwardHash = Animator.StringToHash("Backward");

    private Transform core;
    private Vector3[] points;          // array instead of List
    private int index;
    private Vector3 targetPoint;

    [SerializeField] private float recordInterval = 0.1f;
    private float recordTimer;

    [SerializeField] private float health = 100f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float minDelta = 0.05f;

    [Header("Time rewind")]
    [SerializeField] private int maxStoredStates = 700;
    [SerializeField] private float rewindPlaybackDuration = 1f;
    [SerializeField] private float cornerJitterRadius = 0.05f;

    private Rigidbody rb;
    private Animator animator;

    private bool rewinding;

    private int statesToRewindLeft;
    private float statesPerFrame;
    private float statesStepAccumulator;

    private readonly struct EnemyState
    {
        public readonly Vector3 position;
        public readonly Quaternion rotation;
        public readonly int pathIndex;
        public readonly Vector3 targetPoint;

        public EnemyState(Vector3 pos, Quaternion rot, int idx, Vector3 tgt)
        {
            position = pos;
            rotation = rot;
            pathIndex = idx;
            targetPoint = tgt;
        }
    }

    // ring buffer without extra allocations
    private EnemyState[] history;
    private int historyCount;
    private int historyStart;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        RandomizeAnimation();

        core = GameObject.FindGameObjectWithTag("Core").transform;

        var navPath = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, core.position, NavMesh.AllAreas, navPath);

        // use the corners array directly to avoid List overhead
        points = navPath.corners;

        RandomizePathCorners();

        if (points != null && points.Length > 0)
        {
            index = 0;
            targetPoint = points[index];
        }
        else
        {
            targetPoint = core.position;
        }

        history = new EnemyState[maxStoredStates];
        historyCount = 0;
        historyStart = 0;
    }

    private void RandomizeAnimation()
    {
        if (animator == null)
            return;

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        float random = Random.Range(0f, 1f);
        animator.Play(info.fullPathHash, 0, random);
    }

    private void Update()
    {
        if (rewinding)
            return;

        if (points == null || points.Length == 0)
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
            recordTimer += Time.fixedDeltaTime;
            if (recordTimer >= recordInterval)
            {
                recordTimer -= recordInterval;
                RecordState();
            }

            MoveAlongPath();
        }
    }

    private bool TryProjectToNavMesh(Vector3 src, float maxDistance, out Vector3 result)
    {
        if (NavMesh.SamplePosition(src, out NavMeshHit hit, maxDistance, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = src;
        return false;
    }

    private void RandomizePathCorners()
    {
        if (points == null || points.Length < 2)
            return;

        int last = points.Length - 1;

        for (int i = 1; i < last; i++)
        {
            Vector2 offset = Random.insideUnitCircle * cornerJitterRadius;
            Vector3 jittered = points[i] + new Vector3(offset.x, 0f, offset.y);

            if (TryProjectToNavMesh(jittered, cornerJitterRadius * 2f, out Vector3 snapped))
            {
                points[i] = snapped;
            }
        }
    }

    private void MoveAlongPath()
    {
        if (points == null || points.Length == 0)
            return;

        float distance = Vector3.Distance(transform.position, targetPoint);

        if (distance <= minDelta)
        {
            if (index < points.Length - 1)
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
        int writeIndex;
        if (historyCount < maxStoredStates)
        {
            writeIndex = (historyStart + historyCount) % maxStoredStates;
            historyCount++;
        }
        else
        {
            writeIndex = historyStart;
            historyStart = (historyStart + 1) % maxStoredStates;
        }

        history[writeIndex] = new EnemyState(
            transform.position,
            transform.rotation,
            index,
            targetPoint
        );
    }

    private void RewindStep()
    {
        if (statesToRewindLeft <= 0 || historyCount == 0)
        {
            FinishRewind();
            return;
        }

        statesStepAccumulator += statesPerFrame;
        int stepsThisFrame = Mathf.FloorToInt(statesStepAccumulator);
        if (stepsThisFrame <= 0)
            stepsThisFrame = 1;

        stepsThisFrame = Mathf.Min(stepsThisFrame, statesToRewindLeft, historyCount);

        for (int i = 0; i < stepsThisFrame; i++)
        {
            int lastIndex = (historyStart + historyCount - 1 + maxStoredStates) % maxStoredStates;
            EnemyState state = history[lastIndex];

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

            index = state.pathIndex;
            targetPoint = state.targetPoint;

            historyCount--;
            statesToRewindLeft--;
        }

        statesStepAccumulator -= stepsThisFrame;

        if (statesToRewindLeft <= 0 || historyCount == 0)
        {
            FinishRewind();
        }
    }

    private void FinishRewind()
    {
        rewinding = false;
        if (animator != null)
        {
            animator.SetBool(BackwardHash, false);
            RandomizeAnimation();
        }
    }

    public void ReverseTime(float rewindAmountSeconds)
    {
        if (rewinding)
            return;

        if (historyCount <= 1 || rewindAmountSeconds <= 0f)
            return;

        int availableStates = historyCount;
        float stepDuration = recordInterval > 0f ? recordInterval : Time.fixedDeltaTime;
        int desiredStates = Mathf.RoundToInt(rewindAmountSeconds / stepDuration);

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

        if (animator != null)
        {
            animator.SetBool(BackwardHash, true);
            RandomizeAnimation();
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0f)
            Destroy(gameObject);
    }

    public void Kill()
    {
        OnEnemyKilled?.Invoke();
        Destroy(gameObject);
    }
}
