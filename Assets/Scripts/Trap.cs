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

    bool activated = false;

    public void Awake()
    {
        StartCoroutine(ExplodeAfterSeconds(lifeTime));
    }

    private IEnumerator ExplodeAfterSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        BlowUp();
        Destroy(gameObject);
    }


    void OnTriggerEnter(Collider other)
    {
        //if (other.attachedRigidbody != null &&
        //    other.attachedRigidbody.transform == transform.root)
        //    return;

        //BlowUp();
        //activated = true;
        //Destroy(gameObject);
    }

    private void BlowUp()
    {
        if (activated) return;
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        List<IDamagable> result = new List<IDamagable>();

        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

        foreach (Collider col in colliders)
        {
            IDamagable damagable = col.GetComponent<IDamagable>();
            if (damagable != null)
                damagable.TakeDamage(damage);
        }
    }

    private void OnDestroy()
    {
        BlowUp();
    }
}