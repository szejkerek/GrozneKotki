using System;

public static class GameEvents
{
    public static Action<int> OnScoreChanged;
    public static Action OnGameOver;
}