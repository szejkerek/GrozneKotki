using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.InputSystem;
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

    void Awake()
    {
        core = GameObject.FindGameObjectWithTag("Core").transform;
        path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, core.position, NavMesh.AllAreas, path);
        points = new List<Vector3>(path.corners);
        targetPoint = transform.position;
        initialY = transform.position.y;
    }

    public void RevertTime(float time)
    {
        if(timeReversed != true) StartCoroutine(RevertTimeForSeconds(time));
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


    void Update()
    {
        Keyboard keyboard = Keyboard.current;         

        if (timeReversed)
            MoveBackward();
        else
            MoveForward();
    }

    private void MoveForward()
    {
        if (Vector3.Distance(transform.position, targetPoint) <= minDelta)
        {
            if (index < points.Count - 1)
            {
                index++;
                targetPoint = points[index];
                targetPoint.y = initialY;
            }
        }

        transform.position += (targetPoint - transform.position).normalized * speed * Time.deltaTime;
    }

    private void MoveBackward()
    {
        if (Vector3.Distance(transform.position, targetPoint) <= minDelta)
        {
            if (index > 0)
            {
                index--;
                targetPoint = points[index];
                targetPoint.y = initialY;
            }
        }

        transform.position += (targetPoint - transform.position).normalized * speed * revertModifier * Time.deltaTime;
    }


    

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

}
