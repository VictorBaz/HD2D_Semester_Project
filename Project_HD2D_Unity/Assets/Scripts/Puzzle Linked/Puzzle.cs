using System;
using UnityEngine;
using System.Collections;

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

    private bool _isAlreadyCompleted = false;
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

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;
        
        GameplayEvents.TriggerPuzzleVisited(puzzleID);
    }
}