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

    [Header("Hogward Style")]
    [Tooltip("Jak bardzo pociski są odsunięte od siebie w poziomie")]
    public float lateralOffset = 0.25f;

    [Tooltip("Jak bardzo pociski są odsunięte od siebie w pionie")]
    public float verticalOffset = 0.1f;

    [Tooltip("Jak duży kąt losowego odchylenia (stopnie)")]
    public float angleJitter = 3f;

    protected override void OnUse()
    {
        if (fireballPrefab == null)
        {
            Debug.LogWarning("FastAttackSkill has no fireballPrefab assigned");
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
            // Bazowy kierunek lotu
            Vector3 forward = _player != null ? _player.transform.forward : transform.forward;
            if (forward.sqrMagnitude < 0.0001f)
                forward = Vector3.forward;

            Vector3 right = _player != null ? _player.transform.right : transform.right;
            Vector3 up = _player != null ? _player.transform.up : transform.up;

            // Lekko przesuwamy pocisk w prawo lewo i w górę dół,
            // ale w małym zakresie, żeby dalej wyglądały na równoległe
            float side = Random.Range(-lateralOffset, lateralOffset);
            float vertical = Random.Range(-verticalOffset, verticalOffset);

            Vector3 spawnPos = spawnPoint.position + right * side + up * vertical;

            // Delikatne odchylenie kąta, żeby nie były identyczne, ale wciąż lecą mniej więcej równolegle
            float yaw = Random.Range(-angleJitter, angleJitter);
            float pitch = Random.Range(-angleJitter * 0.5f, angleJitter * 0.5f);

            Quaternion baseRot = Quaternion.LookRotation(forward, Vector3.up);
            Quaternion jitterRot =
                Quaternion.AngleAxis(yaw, Vector3.up) *
                Quaternion.AngleAxis(pitch, right);

            Quaternion finalRot = jitterRot * baseRot;
            Vector3 finalDir = finalRot * Vector3.forward;

            GameObject fireball = Instantiate(
                fireballPrefab,
                spawnPos,
                finalRot
            );

            FireballProjectile projectile = fireball.GetComponent<FireballProjectile>();
            if (projectile != null)
            {
                projectile.Launch(finalDir, fireballSpeed);
            }

            if (i < projectilesInBurst - 1)
                yield return new WaitForSeconds(timeBetweenShots);
        }
    }
}
