using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public int requiredKills = 0;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}