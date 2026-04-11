using System;
using UnityEngine;
using System.Collections;


public class Puzzle : MonoBehaviour
{
    #region Variables
    [Header("Puzzle Info")]
    [SerializeField] private string puzzleID;
    
    [Header("Win Condition")]
    [Tooltip("Le parasite 'Boss' qui valide le puzzle à sa mort")]
    [SerializeField] private Parasite bossParasite;

    [Header("Visual Evolution")] public PuzzleVisuals visuals = new PuzzleVisuals();

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
    private void HandleBossDeath()
    {
        
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
            visuals.ApplyProgress(elapsed / duration);
            yield return null;
        }
        visuals.ApplyProgress(1.0f);
    }
    #endregion

    #region Editor Tools
#if UNITY_EDITOR
    [ContextMenu("Scan Zone For Nature Shaders")]
    private void ScanZone()
    {
        visuals.ScanChildren(transform);
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
    #endregion
}