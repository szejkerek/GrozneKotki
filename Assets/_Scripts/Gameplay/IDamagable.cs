using UnityEngine;

public interface IDamagable
{
    GameObject gameObject { get; }
    void TakeDamage(float damage);
}
