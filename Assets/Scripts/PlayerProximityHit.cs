using UnityEngine;

public class PlayerProximityHit : MonoBehaviour
{
    [Header("Hit effect")]
    public Color hitColor = Color.red;
    public float flashTime = 0.15f;

    [Header("Detection")]
    public float detectionRadius = 2f;
    public LayerMask enemyLayer;
    public float hitCooldown = 0.5f;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackUpward = 0.5f;

    Renderer[] renderers;
    Color[] originalColors;
    bool isFlashing;
    float hitTimer;

    Rigidbody rb;

    void Awake()
    {
        // all renderers on player and children
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        hitTimer -= Time.deltaTime;

        Transform enemy = GetClosestEnemy();

        if (hitTimer <= 0f && enemy != null)
        {
            hitTimer = hitCooldown;
            OnHit(enemy);
        }
    }

    Transform GetClosestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

        if (hits.Length == 0)
            return null;

        float closestSqr = Mathf.Infinity;
        Transform closest = null;

        Vector3 currentPos = transform.position;

        foreach (Collider c in hits)
        {
            Vector3 dir = c.transform.position - currentPos;
            float sqr = dir.sqrMagnitude;

            if (sqr < closestSqr)
            {
                closestSqr = sqr;
                closest = c.transform;
            }
        }

        return closest;
    }

    public void OnHit(Transform enemy)
    {
        // knockback first
        if (rb != null && enemy != null)
        {
            Vector3 direction = (transform.position - enemy.position).normalized;

            // optional  top down knockback, no vertical tilt
            direction.y = 0f;
            direction.Normalize();

            Vector3 force = direction * knockbackForce + Vector3.up * knockbackUpward;
            rb.AddForce(force, ForceMode.Impulse);
        }

        GameplayManager.Instance.TimeBar.SubtractTimeUI(10, byEnemy: false);


        if (!isFlashing)
        {
            StartCoroutine(Flash());
        }
    }

    System.Collections.IEnumerator Flash()
    {
        isFlashing = true;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                renderers[i].material.color = hitColor;
        }

        yield return new WaitForSeconds(flashTime);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                renderers[i].material.color = originalColors[i];
        }

        isFlashing = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
