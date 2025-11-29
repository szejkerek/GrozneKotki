using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class timeBar : MonoBehaviour
{
    public float timeMax = 100;
    public float timeLeft = 10;
    public Image bar;
    public RectTransform hourglass;

    public void Update()
    {
        bar.fillAmount = timeLeft/timeMax;
        var keyboard = Keyboard.current;

        if (keyboard.rKey.wasPressedThisFrame)
            FlipHourglass();


    }

    public void AddTimeUI(float amount)
    {
        timeLeft = Math.Clamp(timeLeft + amount, 0, timeMax);
    }

    public void SubtractTimeUI(float amount)
    {
        timeLeft = Math.Clamp(timeLeft-amount, 0, timeMax);
    }

    public void SetTimePercentUI(float percent)
    {
        timeLeft = percent * timeMax;
    }

    public void FlipHourglass()
    {
        hourglass.DORotate(new Vector3(0, 0, 180), 0.4f, RotateMode.Fast);
    }


}
