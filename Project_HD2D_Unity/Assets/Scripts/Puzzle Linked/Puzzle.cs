using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Puzzle : MonoBehaviour
{
    #region Variables
    [Header("Puzzle Info")]
    [SerializeField] private string puzzleID;
    public string PuzzleID => puzzleID;
    
    [SerializeField] private Transform spawnPoint;
    public Transform SpawnPoint => spawnPoint;
    
    [Header("Win Condition")]
    [Tooltip("Le parasite 'Boss' qui valide le puzzle à sa mort")]
    [SerializeField] private Parasite bossParasite;

    [Header("Visual Evolution")] 
    public PuzzleVisuals visuals = new();

    [Header("Linked Components")]
    [Tooltip("Le contrôleur externe qui gère le fondu du Post-Process")]
    [SerializeField] private VolumeFadeController volumeFadeController;
    

    private bool _isAlreadyCompleted = false;
    public bool IsAlreadyCompleted => _isAlreadyCompleted;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        visuals.Initialize();
    }

    private void OnEnable()
    {
        if (bossParasite != null)
        {
            bossParasite.OnDeath += HandleBossDeath;
        }
    }

    private void OnDisable()
    {
        if (bossParasite != null)
        {
            bossParasite.OnDeath -= HandleBossDeath;
        }
    }
    #endregion

    #region Logic
    public void SetCompletedState(bool isCompleted)
    {
        _isAlreadyCompleted = isCompleted;

        if (isCompleted)
        {
            visuals.ApplyProgress(0f);
            
            if (volumeFadeController != null)
            {
                volumeFadeController.SetInstantWeight(0f, false);
            }

            if (bossParasite != null)
            {
                bossParasite.gameObject.SetActive(false);
            }
        }
    }

    private void HandleBossDeath(Parasite parasite)
    {
        if (_isAlreadyCompleted) return;
        
        CompletePuzzle();
    }

    public void CompletePuzzle()
    {
        _isAlreadyCompleted = true;
        
        if (volumeFadeController != null)
        {
            volumeFadeController.TriggerVolumeFade(0f);
        }

        foreach (EnemyBaseManager enemy in visuals.Enemies)
        {
            enemy.ChangeState(enemy.FriendlyState);
        }
        
        StartCoroutine(AnimateEnvironment());
        GameplayEvents.TriggerPuzzleCompleted(puzzleID);
    }

    private IEnumerator AnimateEnvironment()
    {
        float elapsed = 0;
        float duration = 2.0f; 
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            visuals.ApplyProgress(1 - elapsed / duration);
            yield return null;
        }
        visuals.ApplyProgress(0f);
    }
    #endregion

    #region Volume Controller Communication
    public void NotifyPlayerEnter()
    {
        this.TriggerPuzzleVisited();
    }

    public void ChangeSpawnPoint(Transform spawnPoint)
    {
        this.spawnPoint = spawnPoint;
    }

    public void NotifyPlayerExit()
    {
    }
    #endregion
}