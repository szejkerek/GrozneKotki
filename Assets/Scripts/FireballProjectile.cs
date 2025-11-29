using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
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

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;  
        rb.isKinematic = false;  
    }

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

        Vector3 newPos = rb.position + direction * speed * Time.deltaTime;
        rb.MovePosition(newPos);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.attachedRigidbody != null &&
            collision.collider.attachedRigidbody.transform == transform.root)
            return;
        
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
        
        if (isArea)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

            foreach (Collider col in colliders)
            {
                ApplyEffects(col);
            }
        }
        else
        {
            ApplyEffects(collision.collider);
        }
        
        Destroy(gameObject);
    }

    void ApplyEffects(Collider other)
    {
        if(other.TryGetComponent<IDamagable>(out var damagable))
        {
            damagable.TakeDamage(damage);
            
            Enemy enemy = damagable.gameObject.GetComponent<Enemy>();
            if (enemy != null && revertsTime)
            {
                enemy.RevertTime(revertTimeTime);
            }
        }
    }
}
