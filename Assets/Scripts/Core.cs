using System;
using UnityEngine;

public class Core : MonoBehaviour, IDamagable
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Taking damage");
    }
}