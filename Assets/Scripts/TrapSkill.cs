using UnityEngine;

public class TrapSkill : PlayerSkill
{
    [Header("Trap")]
    public GameObject trap;

    protected override void OnUse()
    {
        if (trap == null)
        {
            Debug.LogWarning("TrapSkill has no trapPrefab assigned");
            return;
        }

        Transform shootingPoint = _player.spawnPoint;
        
        Vector3 direction = _player.transform.forward;
        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector3.forward;

        GameObject fireball = Instantiate(
            trap,
            shootingPoint.position,
            Quaternion.LookRotation(direction, Vector3.up)
        );
    }
}