using System;
using UnityEngine;

public abstract class PlayerSkill : MonoBehaviour
{
    protected Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    public float cooldown = 1f;

    float lastUseTime = Mathf.NegativeInfinity;
    
    

    public void TryUse()
    {
        if (Time.time < lastUseTime + cooldown)
            return;

        lastUseTime = Time.time;
        OnUse();
    }

    protected abstract void OnUse();
}

