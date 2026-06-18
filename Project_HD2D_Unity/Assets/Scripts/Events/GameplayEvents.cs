using System;
using UnityEngine;
using UnityEngine.InputSystem;

public static class GameplayEvents
{
    /// <summary> Appelé quand un boss de zone est vaincu. Paramètre : ID du puzzle </summary>
    public static event Action<string> OnPuzzleCompleted;

    public static void TriggerPuzzleCompleted(string puzzleID) => OnPuzzleCompleted?.Invoke(puzzleID);
    
    public static event Action<string> OnPuzzleVisited;
    public static void TriggerPuzzleVisited(string puzzleID) => OnPuzzleVisited?.Invoke(puzzleID);
    
    public static Action<Vector3> OnCheckpoint;
    public static void TriggerCheckpoint(Vector3 checkpoint) => OnCheckpoint?.Invoke(checkpoint);

    public static event Action<float> OnCredits;
    public static void TriggerCredits(float credits) => OnCredits?.Invoke(credits);
    
    public static event Action <bool> OnPlayerBlocked;
    public static void TriggerPlayerEnable(bool block) => OnPlayerBlocked?.Invoke(block);
    
    public static event Action OnSkip;
    public static void TriggerSkip(InputAction.CallbackContext ctx) => OnSkip?.Invoke();
    
}