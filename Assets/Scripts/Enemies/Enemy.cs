using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;

public class Enemies : MonoBehaviour, IDamagable
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

    void Awake()
    {
        core = GameObject.FindGameObjectWithTag("Core").transform;
        path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, core.position, NavMesh.AllAreas, path);
        points = new List<Vector3>(path.corners);
        targetPoint = transform.position;
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard.rKey.wasPressedThisFrame)
        {
            index--;
            targetPoint = points[index];
            targetPoint.y = 0;
        }
            
        else if (keyboard.rKey.wasReleasedThisFrame)
        {
            index++;
            targetPoint = points[index];
            targetPoint.y = 0;
        }
            

        if (keyboard.rKey.isPressed)
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
                targetPoint.y = 0;
            }
        }

        transform.position += (targetPoint - transform.position).normalized * speed * Time.deltaTime;

        for (int i = index; i < points.Count - 1; i++)
        {
            Debug.DrawLine(points[i], points[i + 1], Color.red);
        }
    }

    private void MoveBackward()
    {
        if (Vector3.Distance(transform.position, targetPoint) <= minDelta)
        {
            if (index > 0)
            {
                index--;
                targetPoint = points[index];
                targetPoint.y = 0;
            }
        }

        for (int i = index; i > 1; i--)
        {
            Debug.DrawLine(points[i], points[i - 1], Color.red);
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
