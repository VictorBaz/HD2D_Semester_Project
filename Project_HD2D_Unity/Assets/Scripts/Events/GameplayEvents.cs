using System;

public static class GameplayEvents
{
    /// <summary> Appelé quand un boss de zone est vaincu. Paramètre : ID du puzzle </summary>
    public static event Action<string> OnPuzzleCompleted;

    public static void TriggerPuzzleCompleted(string puzzleID) => OnPuzzleCompleted?.Invoke(puzzleID);
}