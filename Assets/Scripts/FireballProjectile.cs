using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FireballProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 3f;
    public float damage = 10f;
    public GameObject hitEffect;

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

        Destroy(gameObject);
    }
}