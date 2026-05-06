using System;
using UnityEngine;

public static class GameplayEvents
{
    /// <summary> Appelé quand un boss de zone est vaincu. Paramètre : ID du puzzle </summary>
    public static event Action<string> OnPuzzleCompleted;

    public static void TriggerPuzzleCompleted(string puzzleID) => OnPuzzleCompleted?.Invoke(puzzleID);
    
    public static event Action<string> OnPuzzleVisited;
    public static void TriggerPuzzleVisited(string puzzleID) => OnPuzzleVisited?.Invoke(puzzleID);
    
    public static Action<Vector3> OnCheckpoint;
    public static void TriggerCheckpoint(Vector3 checkpoint) => OnCheckpoint?.Invoke(checkpoint);
}