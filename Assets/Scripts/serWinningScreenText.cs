using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using TMPro;

public class serWinningScreenText : MonoBehaviour
{
    public TMP_Text timeItTookSumTextfield;
    public TMP_Text amountofGhostsTextfield;

    public string timeItTookSum;
    public string amountofGhosts;

    public void Start()
    {
        FillText();
    }

    public void FillText()
    {
        timeItTookSumTextfield.text = timeItTookSum + " s";
        amountofGhostsTextfield.text = amountofGhosts + " helpers";
    }




}
