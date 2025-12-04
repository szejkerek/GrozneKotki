using System;
using UnityEngine;

public abstract class PlayerSkill : MonoBehaviour
{
    protected Player _player;
    [SerializeField] protected PowerupCooldownUI cooldownUI;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    public float cooldown = 1f;

    float lastUseTime = Mathf.NegativeInfinity;
    
    public bool TryUse()
    {
        if (Time.time < lastUseTime + cooldown)
            return false;

        lastUseTime = Time.time;
        OnUse();
        return true;
    }

    private void Update()
    {
        if(cooldownUI == null) return;
        cooldownUI.SetTimePercentUI(Mathf.Min(1, (Time.time - lastUseTime) /cooldown));
    }

    protected abstract void OnUse();
}

