using System.Collections;
using UnityEngine;

public class FastAttackSkill : PlayerSkill
{
    [Header("Fireball")]
    public GameObject fireballPrefab;
    public float fireballSpeed = 15f;

    [Header("Burst")]
    public int projectilesInBurst = 3;
    public float timeBetweenShots = 0.12f;

    protected override void OnUse()
    {
        if (fireballPrefab == null)
        {
            Debug.LogWarning("TripleFireballSkill has no fireballPrefab assigned");
            return;
        }

        StartCoroutine(FireBurst());
    }

    IEnumerator FireBurst()
    {
        Transform spawnPoint = _player != null && _player.spawnPoint != null
            ? _player.spawnPoint
            : transform;

        for (int i = 0; i < projectilesInBurst; i++)
        {
            Vector3 direction = _player != null ? _player.transform.forward : transform.forward;
            if (direction.sqrMagnitude < 0.0001f)
                direction = Vector3.forward;

            GameObject fireball = Instantiate(
                fireballPrefab,
                spawnPoint.position,
                Quaternion.LookRotation(direction, Vector3.up)
            );

            FireballProjectile projectile = fireball.GetComponent<FireballProjectile>();
            if (projectile != null)
            {
                projectile.Launch(direction, fireballSpeed);
            }

            if (i < projectilesInBurst - 1)
                yield return new WaitForSeconds(timeBetweenShots);
        }
    }
}