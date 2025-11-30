using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FireTrap : MonoBehaviour
{
    public float lifeTime = 6f;
    public float damage = 80f;
    public float blastRadius = 4f;
    public bool debugDamage = true;

    bool activated;

    void Awake()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

        foreach (Collider col in colliders)
        {
            IDamagable damagable = col.GetComponent<IDamagable>();
            if (damagable != null)
            {
                float frameDamage = damage * Time.deltaTime;
                damagable.TakeDamage(frameDamage);

#if UNITY_EDITOR
                if (debugDamage)
                {
                    Debug.Log(
                        $"[FireTrap] {name} dealt {frameDamage:F2} damage to {col.name}",
                        this
                    );
                }
#endif
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Filled sphere for visual area
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.15f);
        Gizmos.DrawSphere(transform.position, blastRadius);

        // Wireframe outline
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}