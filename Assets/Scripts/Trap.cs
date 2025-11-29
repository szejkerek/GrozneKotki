using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Trap : MonoBehaviour
{
    public float lifeTime = 3f;
    public float damage = 40f;
    public GameObject hitEffect;

    public void Awake()
    {
        Destroy(gameObject, lifeTime);
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

        if(other.GetComponent<IDamagable>() != null)
        {
            other.GetComponent<IDamagable>().TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}