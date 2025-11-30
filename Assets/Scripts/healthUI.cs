using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class healthUI : MonoBehaviour
{
    public List<Image> hearts;
    public Color active;
    public Color inactive;
    public int activeCount;
    public int maxHealth;
    public Vector3 initialTransform;

    public void Start()
    {
        initialTransform = this.transform.position;
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y-500, this.transform.position.z);
    }

    private void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard.oKey.wasPressedThisFrame)
            LoadHealthDisplay();
    }

    public void LoadHealthDisplay()
    {
        this.transform.DOMove(initialTransform, 0.3f);
    }

    public void FullHealthUI()
    {
        activeCount = maxHealth;

        foreach(Image im in hearts)
        {
            im.color = active;
        }
    }

    public void subtractHealthUI()
    {
        activeCount = Math.Clamp(activeCount - 1, 0, maxHealth);
        hearts[activeCount].color = inactive;
    }
    
    public void addHealthUI()
    {
        activeCount = Math.Clamp(activeCount + 1, 0, maxHealth);
        hearts[activeCount].color = active;
    }


}
