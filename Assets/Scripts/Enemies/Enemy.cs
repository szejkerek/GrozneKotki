using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class Enemies : MonoBehaviour
{
    private Transform core;
    private NavMeshPath path;
    Queue<Vector3> points;
    Stack<Vector3> usedPoints;
    Vector3 targetPoint;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float revertModifier = 2f;
    float minDelta = 0.1f;
    float distanceFromCore = 0.5f;

    void Awake()
    {
        core = GameObject.FindGameObjectWithTag("Core").transform;
        path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, core.position, NavMesh.AllAreas, path);
        points = new Queue<Vector3>(path.corners);
        targetPoint = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown("R"))
        {
            SetPreviousPoint();
        }
        else if (Input.GetKeyUp("R"))
        {
            SetNextPoint();
        }
        if (Input.GetKey("R")) MoveBackward();
        else MoveForward();
    }

    private void SetNextPoint()
    {
        usedPoints.Push(targetPoint);
        targetPoint = points.Dequeue();
        targetPoint.y = transform.position.y;
    }

    private void SetPreviousPoint()
    {
        points.Enqueue(targetPoint);
        targetPoint = usedPoints.Pop();
        targetPoint.y = transform.position.y;
    }

    private void MoveForward()
    {
        if (Vector3.Distance(transform.position, targetPoint) <= minDelta)
        {
            if (points.Count > 0)
            {
                SetNextPoint();               
            }
        }

        transform.position += (targetPoint - transform.position).normalized * speed * Time.deltaTime;
    }

    private void MoveBackward()
    {
        if (Vector3.Distance(transform.position, targetPoint) <= minDelta)
        {
            if (usedPoints.Count > 0)
            {
                SetPreviousPoint();
            }
        }

        transform.position += (targetPoint - transform.position).normalized * speed * revertModifier * Time.deltaTime;
    }

}
