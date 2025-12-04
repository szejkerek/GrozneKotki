using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Trap : MonoBehaviour
{
    public float lifeTime = 3f;
    public float damage = 40f;
    public float blastRadius = 3f;
    public GameObject hitEffect;

    bool activated;

    void Awake()
    {
        StartCoroutine(ExplodeAfterSeconds(lifeTime));
    }

    IEnumerator ExplodeAfterSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        BlowUp();
    }

    void OnTriggerEnter(Collider other)
    {
        //if (other.attachedRigidbody != null &&
        //    other.attachedRigidbody.transform == transform.root)
        //    return;

        BlowUp();
    }

    void BlowUp()
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
                damagable.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        // zostaw puste albo usuń cały OnDestroy,
        // ważne aby nie wywoływać tu BlowUp
    }
}