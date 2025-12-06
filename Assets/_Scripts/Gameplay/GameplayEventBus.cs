using System;
using UnityEngine;

public class GameplayEventBus : MonoBehaviour
{
    public static event Action OnPlayerDied;
    public static event Action<int> OnScoreChanged;

    public static void RaisePlayerDied()
    {
        OnPlayerDied?.Invoke();
    }

    public static void RaiseScoreChanged(int newScore)
    {
        OnScoreChanged?.Invoke(newScore);
    }
}
