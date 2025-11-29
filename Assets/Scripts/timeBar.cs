using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using TMPro;

public class timeBar : MonoBehaviour
{
    public float timeMax = 100;
    public float timeLeft = 10;
    public Image bar;
    public RectTransform hourglass;
    public TMP_Text displayedTime;
    public TMP_Text displayedSubtractTime;

    float _time;
    float _interval = 1;

    private void Start()
    {
        timeLeft = timeMax;
        displayedSubtractTime.gameObject.SetActive(false);
    }

    public void Update()
    {
        _time += Time.deltaTime;
        while (_time >= _interval)
        {
            SubtractTimeUI(_interval, false);
            _time -= _interval;
        }

    }

    public void AddTimeUI(float amount)
    {
        FlipHourglass(false);
        SetTimePercentUI(Math.Clamp(timeLeft + amount, 0, timeMax) / timeMax);
    }

    public void SubtractTimeUI(float amount, bool byEnemy)
    {
        displayedTime.text = Mathf.Round(timeLeft * 1.0f) + "s";
        SetTimePercentUI(Math.Clamp(timeLeft - amount, 0, timeMax)/timeMax);
        if (byEnemy)
        {
            FlipHourglass(true);
            ShowWithPunch(displayedSubtractTime);
        }
    }

    public void SetTimePercentUI(float percent)
    {
        timeLeft = percent * timeMax; //???????????
        bar.DOFillAmount(timeLeft / timeMax, .3f).SetEase(Ease.Linear);
    }

    public void FlipHourglass(bool back)
    {
        if (back)
        {
            //hourglass.Rotate(Vector3.zero);
            hourglass.DORotate(hourglass.eulerAngles + new Vector3(0, 0, 180), 0.4f, RotateMode.Fast);
        }
        else
        {
            //hourglass.Rotate(Vector3.zero);
            hourglass.DORotate(hourglass.eulerAngles + new Vector3(0, 0, 180), 0.4f, RotateMode.Fast);
        }

    }

    public void ShowWithPunch(TMP_Text text)
    {
        text.DOKill(true);
        text.transform.DOKill(true);

        text.gameObject.SetActive(true);
        text.alpha = 1f;
        text.transform.localScale = Vector3.one;

        // Punch
        text.transform.DOPunchScale(
            punch: new Vector3(0.3f, 0.3f, 0f),
            duration: 0.4f,
            vibrato: 10
        );

        DOVirtual.DelayedCall(1.5f, () =>
        {
            text.DOFade(0f, 0.4f)
                    .OnComplete(() => text.gameObject.SetActive(false));
        });
    }


}
