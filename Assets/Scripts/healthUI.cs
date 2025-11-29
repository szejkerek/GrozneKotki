using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthUI : MonoBehaviour
{
    public List<Image> hearts;
    public Color active;
    public Color inactive;
    public int activeCount;
    public int maxHealth;

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
