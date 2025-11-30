using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FireTrap : MonoBehaviour
{
    public float lifeTime = 6f;
    public float damage = 80f;
    public float blastRadius = 4f;
    public GameObject hitEffect;

    bool activated;

    void Awake()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (activated) return;
        activated = true;

        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

        foreach (Collider col in colliders)
        {
            IDamagable damagable = col.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(damage*Time.deltaTime);
            }
        }
    }

}