using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PowerupCooldownUI : MonoBehaviour
{
    public float timeLeft = 10;
    public Image bar;
    public Image icon;
    public Color color;
    //public RectTransform hourglass;
    //public TMP_Text displayedTime;
    //public TMP_Text displayedSubtractTime;

    float _time;
    float _interval = 1;

    private void Start()
    {
        timeLeft = 1;
        bar.DOColor(color, 0.3f);
        icon.DOColor(color, 0.3f);
        //displayedSubtractTime.gameObject.SetActive(false);
    }

    public void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard.oKey.wasPressedThisFrame)
            PowerupUsedUI(3);

        _time += Time.deltaTime;
        while (_time >= _interval)
        {
            AddTimeUI(.1f);
            _time -= _interval;
        }

    }

    public void AddTimeUI(float amount)
    {
        SetTimePercentUI(Math.Clamp(timeLeft + amount, 0, 1) / 1);
        if(timeLeft == 1)
        {
            bar.DOColor(color, 0.3f);
            icon.DOColor(color, 0.3f);
        }
    }

    public void PowerupUsedUI(float amount)
    {
        SetTimePercentUI(0);
        Color c = color;
        c = new Color(c.r * 0.5f, c.g * 0.5f, c.b * 0.5f, c.a*.9f);

        bar.DOColor(c, 0.3f);
        icon.DOColor(c, 0.3f);
    }

    public void SetTimePercentUI(float percent)
    {
        timeLeft = percent;
        bar.DOFillAmount(timeLeft / 1, .3f).SetEase(Ease.Linear);
    }


    public void ShowWithPunch(TMP_Text text)
    {
        text.gameObject.SetActive(true);
        //text.alpha = 1f;
        text.transform.localScale = Vector3.one;

        // Punch
        text.transform.DOPunchScale(
            punch: new Vector3(0.3f, 0.3f, 0f),
            duration: 0.4f,
            vibrato: 10
        );

        DOVirtual.DelayedCall(1.5f, () =>
        {
            text.DOFade(0f, 0.3f)
                    .OnComplete(() => text.gameObject.SetActive(false));
        });
    }
}
