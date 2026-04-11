using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private List<string> completedPuzzles = new();

    private void OnEnable()
    {
        GameplayEvents.OnPuzzleCompleted += RegisterPuzzleCompletion;
    }

    private void OnDisable()
    {
        GameplayEvents.OnPuzzleCompleted -= RegisterPuzzleCompletion;
    }

    private void RegisterPuzzleCompletion(string id)
    {
        if (!completedPuzzles.Contains(id))
        {
            completedPuzzles.Add(id);
        }
    }
}