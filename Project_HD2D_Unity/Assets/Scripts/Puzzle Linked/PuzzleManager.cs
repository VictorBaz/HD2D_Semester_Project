using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private List<Puzzle> puzzles = new();
    [SerializeField] private List<string> completedPuzzles = new();

    public static PuzzleManager Instance;

    public string LastPuzzleCompleted()
    {
        if (completedPuzzles.Count == 0)
        {
            return "";
        }
        return completedPuzzles[^1];
    }
    
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

    #region IDataPersistence

    public void LoadData(GameData data)
    {
        completedPuzzles.Clear();
        if (data.CompletedPuzzles != null)
        {
            completedPuzzles = new List<string>(data.CompletedPuzzles);
        }

        RefreshPuzzlesInScene();
    }

    public void SaveData(ref GameData data)
    {
        data.CompletedPuzzles = new List<string>(completedPuzzles);
        data.LastCompletedPuzzleId = LastPuzzleCompleted();
    }

    #endregion

    private void RefreshPuzzlesInScene()
    {
        foreach (Puzzle puzzle in puzzles)
        {
            if (puzzle == null) continue;

            bool isDone = completedPuzzles.Contains(puzzle.PuzzleID);
            puzzle.SetCompletedState(isDone);
        }
    }
    
    public Puzzle GetPuzzleById(string puzzleID)
    {
        foreach (var puzzle in puzzles)
        {
            if (puzzle != null && puzzle.PuzzleID == puzzleID)
                return puzzle;
        }
        return null;
    }
}