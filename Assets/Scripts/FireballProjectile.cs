using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FireballProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 3f;
    public float damage = 40f;
    public GameObject hitEffect;

    public bool revertsTime = false;
    public float revertTimeTime = 3f;

    public bool isArea = false;
    public float blastRadius = 3f;

    Vector3 direction;
    bool launched;

    public void Launch(Vector3 dir, float overrideSpeed = 0f)
    {
        direction = dir.normalized;
        if (overrideSpeed > 0f)
            speed = overrideSpeed;

        launched = true;
        
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (!launched)
            return;

        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null &&
            other.attachedRigidbody.transform == transform.root)
            return;

        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        if(isArea)
        {
            List<IDamagable> result = new List<IDamagable>();

            Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

            foreach (Collider col in colliders)
            {
                ApplyEffects(col);
            }
        }
        else
        {
            ApplyEffects(other);
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void ApplyEffects(Collider other)
    {
        if (other.GetComponent<IDamagable>() != null)
        {
            other.GetComponent<IDamagable>().TakeDamage(damage);
        }

        if (other.GetComponent<Enemy>() != null)
        {
            if (revertsTime)
            {
                other.GetComponent<Enemy>().RevertTime(revertTimeTime);
            }
        }
    }
}