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

    
    [SerializeField] Button mainmenu;
    public string timeItTookSum;
    public string amountofGhosts;

    public void OnEnable()
    {
        FillText();
        Time.timeScale = 0;
        mainmenu.onClick.AddListener(() => Bootstrap.Instance.SceneManager.LoadScene("MainMenu"));
    }

    public void FillText()
    {
        timeItTookSumTextfield.text =  Time.realtimeSinceStartup + " s";
        amountofGhostsTextfield.text = Bootstrap.Instance.GhostRunManager.completedRuns.Count + " helpers";
    }
    
    




}
