using System;
using System.Collections.Generic;
using UnityEngine;


public class PuzzleManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private List<Puzzle> puzzles = new();
    
    [SerializeField] private List<string> completedPuzzles = new();

    public string LastPuzzleCompleted()
    {
        if (completedPuzzles.Count == 0)
        {
            return "";
        }
        return completedPuzzles[^1];
    }
    
    public static PuzzleManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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

    public void LoadData(GameData data)
    {
        completedPuzzles.Clear();

        if (data.CompletedPuzzles is { Count: > 0 })
        {
            completedPuzzles = new List<string>(data.CompletedPuzzles);
        }
    }

    public void SaveData(ref GameData data)
    {
        data.CompletedPuzzles = new List<string>(completedPuzzles);
        data.LastCompletedPuzzleId = LastPuzzleCompleted();
    }
    
    public Puzzle GetPuzzleById(string puzzleID)
    {
        foreach (var puzzle in puzzles)
        {
            if (puzzle.PuzzleID == puzzleID)
            {
                Debug.Log("Puzzle Found: " + puzzle.PuzzleID);
                return puzzle;
            }
        }
        Debug.Log("Puzzle Not Found");
        return null;
    }
}