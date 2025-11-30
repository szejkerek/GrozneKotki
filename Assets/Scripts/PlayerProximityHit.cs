using System.Collections;
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

    public int maxColliders = 8;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackUpward = 0.5f;

    Renderer[] renderers;
    Color[] originalColors;
    bool isFlashing;
    float hitTimer;

    Rigidbody rb;

    Collider[] overlapResults;

    WaitForSeconds flashWait;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && renderers[i].material != null)
            {
                originalColors[i] = renderers[i].material.color;
            }
        }

        rb = GetComponent<Rigidbody>();

        overlapResults = new Collider[maxColliders];
        flashWait = new WaitForSeconds(flashTime);
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
        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            detectionRadius,
            overlapResults,
            enemyLayer
        );

        if (count == 0)
            return null;

        float closestSqr = Mathf.Infinity;
        Transform closest = null;

        Vector3 currentPos = transform.position;

        for (int i = 0; i < count; i++)
        {
            Collider c = overlapResults[i];
            if (c == null)
                continue;

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
        if (rb != null && enemy != null)
        {
            Vector3 direction = (transform.position - enemy.position).normalized;

            direction.y = 0f;
            direction.Normalize();

            Vector3 force = direction * knockbackForce + Vector3.up * knockbackUpward;
            rb.AddForce(force, ForceMode.Impulse);
        }

        GameplayManager.Instance.TimeBar.SubtractTimeUI(10, byEnemy: false);

        if (!isFlashing && gameObject.activeInHierarchy)
        {
            StartCoroutine(Flash());
        }
    }

    IEnumerator Flash()
    {
        isFlashing = true;

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer r = renderers[i];
            if (r == null || r.material == null)
                continue;

            r.material.color = hitColor;
        }

        yield return flashWait;

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer r = renderers[i];
            if (r == null || r.material == null)
                continue;

            r.material.color = originalColors[i];
        }

        isFlashing = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
