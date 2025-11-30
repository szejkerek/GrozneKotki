using System;
using UnityEngine;

public class MonsterCounter : MonoBehaviour
{
    private int counter = 0;
    private int max = 0;
    [SerializeField] TMPro.TMP_Text text;
    private void Start()
    {
        max = GameplayManager.Instance.MaxEnemy;
        Enemy.OnEnemyKilled += OnKilled;
        text.text = $"{max-counter} time devils";
    }

    private void OnDisable()
    {
        Enemy.OnEnemyKilled -= OnKilled;
    }

    private void OnKilled()
    {
        counter++;
        text.text = $"{max-counter} time devils";
        
    }
}
