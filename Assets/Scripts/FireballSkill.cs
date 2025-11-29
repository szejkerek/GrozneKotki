using UnityEngine;

public class FireballSkill : PlayerSkill
{
    [Header("Fireball")]
    public GameObject fireballPrefab;
    public float fireballSpeed = 15f;

    protected override void OnUse()
    {
        if (fireballPrefab == null)
        {
            Debug.LogWarning("FireballSkill has no fireballPrefab assigned");
            return;
        }

        // ShootingPoint przypisujesz w inspectorze do _player.spawnPoint
        Transform shootingPoint = _player.spawnPoint;
        
        Vector3 direction = _player.transform.forward;
        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector3.forward;

        GameObject fireball = Instantiate(
            fireballPrefab,
            shootingPoint.position,
            Quaternion.LookRotation(direction, Vector3.up)
        );

        FireballProjectile projectile = fireball.GetComponent<FireballProjectile>();
        if (projectile != null)
        {
            projectile.Launch(direction, fireballSpeed);
        }
    }
}