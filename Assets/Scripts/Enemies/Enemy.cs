using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class Enemy : MonoBehaviour, IDamagable
{
    private Transform core;
    private NavMeshPath path;
    List<Vector3> points;
    public int index = 0;
    Vector3 targetPoint;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float revertModifier = 2f;
    [SerializeField] float minDelta = 0.05f;
    [SerializeField] float health = 100f;

    float initialY;
    bool timeReversed = false;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        core = GameObject.FindGameObjectWithTag("Core").transform;

        path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, core.position, NavMesh.AllAreas, path);
        points = new List<Vector3>(path.corners);

        targetPoint = transform.position;
        initialY = transform.position.y;
    }

    public void RevertTime(float time)
    {
        if (!timeReversed)
            StartCoroutine(RevertTimeForSeconds(time));
    }

    private IEnumerator RevertTimeForSeconds(float time)
    {
        if (index > 0)
        {
            index--;
            targetPoint = points[index];
            targetPoint.y = initialY;
        }

        timeReversed = true;

        yield return new WaitForSeconds(time);

        timeReversed = false;

        if (index < points.Count - 1)
        {
            index++;
            targetPoint = points[index];
            targetPoint.y = initialY;
        }
    }

    void FixedUpdate()
    {
        if (timeReversed)
            MoveBackward();
        else
            MoveForward();
    }

    private void MoveForward()
    {
        Vector3 currentPos = rb.position;

        if (Vector3.Distance(currentPos, targetPoint) <= minDelta)
        {
            if (index < points.Count - 1)
            {
                index++;
                targetPoint = points[index];
                targetPoint.y = initialY;
            }
        }

        Vector3 dir = (targetPoint - currentPos).normalized;
        Vector3 nextPos = currentPos + dir * speed * Time.fixedDeltaTime;
        nextPos.y = initialY;

        rb.MovePosition(nextPos);
    }

    private void MoveBackward()
    {
        Vector3 currentPos = rb.position;

        if (Vector3.Distance(currentPos, targetPoint) <= minDelta)
        {
            if (index > 0)
            {
                index--;
                targetPoint = points[index];
                targetPoint.y = initialY;
            }
        }

        Vector3 dir = (targetPoint - currentPos).normalized;
        Vector3 nextPos = currentPos + dir * speed * revertModifier * Time.fixedDeltaTime;
        nextPos.y = initialY;

        rb.MovePosition(nextPos);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
