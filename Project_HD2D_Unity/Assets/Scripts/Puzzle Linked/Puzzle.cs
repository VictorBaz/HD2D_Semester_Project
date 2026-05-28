using System;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

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

    [Header("Post Process")]
    [SerializeField] private Volume localVolume;
    [SerializeField] private float fadeDuration = 0.5f;

    private bool _isAlreadyCompleted = false;
    private Coroutine _volumeFadeCoroutine;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        visuals.Initialize();

        if (localVolume != null)
        {
            localVolume.isGlobal = false;
            localVolume.weight = 0f;
        }
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
            
            if (localVolume != null)
            {
                localVolume.isGlobal = false;
                localVolume.weight = 0f;
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
        
        TriggerVolumeFade(0f);

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

    private void TriggerVolumeFade(float targetWeight)
    {
        if (localVolume == null) return;

        if (_volumeFadeCoroutine != null)
        {
            StopCoroutine(_volumeFadeCoroutine);
        }
        _volumeFadeCoroutine = StartCoroutine(FadeVolumeWeightRoutine(targetWeight));
    }

    private IEnumerator FadeVolumeWeightRoutine(float targetWeight)
    {
        float elapsed = 0f;
        float startWeight = localVolume.weight;

        if (targetWeight > 0f)
        {
            localVolume.isGlobal = true;
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            localVolume.weight = Mathf.Lerp(startWeight, targetWeight, elapsed / fadeDuration);
            yield return null;
        }

        localVolume.weight = targetWeight;

        if (targetWeight <= 0f)
        {
            localVolume.isGlobal = false;
        }

        _volumeFadeCoroutine = null;
    }
    #endregion

    #region Triggers
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("A");
        if (_isAlreadyCompleted) return;
        Debug.Log("B");
        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;
        Debug.Log("C");
        
        TriggerVolumeFade(1f);

        GameplayEvents.TriggerPuzzleVisited(puzzleID);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_isAlreadyCompleted) return;

        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;

        TriggerVolumeFade(0f);
    }
    #endregion
}