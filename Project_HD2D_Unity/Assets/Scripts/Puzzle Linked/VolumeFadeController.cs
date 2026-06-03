using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumeFadeController : MonoBehaviour
{
    [Header("Post Process Settings")]
    [SerializeField] private Volume volume;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Linked Puzzle (Optional)")]
    [Tooltip("Référence vers le puzzle associé pour lui envoyer les signaux de trigger")]
    [SerializeField] private Puzzle linkedPuzzle;

    private Coroutine _volumeFadeCoroutine;

    private void Awake()
    {
        if (volume != null)
        {
            volume.isGlobal = false;
            volume.weight = 0f;
        }
    }

    public void SetInstantWeight(float targetWeight, bool isGlobal)
    {
        if (volume == null) return;

        if (_volumeFadeCoroutine != null)
        {
            StopCoroutine(_volumeFadeCoroutine);
            _volumeFadeCoroutine = null;
        }

        volume.isGlobal = isGlobal;
        volume.weight = targetWeight;
    }

    public void TriggerVolumeFade(float targetWeight)
    {
        if (volume == null) return;

        if (_volumeFadeCoroutine != null)
        {
            StopCoroutine(_volumeFadeCoroutine);
        }
        _volumeFadeCoroutine = StartCoroutine(FadeVolumeWeightRoutine(targetWeight));
    }

    private IEnumerator FadeVolumeWeightRoutine(float targetWeight)
    {
        float elapsed = 0f;
        float startWeight = volume.weight;

        if (targetWeight > 0f)
        {
            volume.isGlobal = true;
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            volume.weight = Mathf.Lerp(startWeight, targetWeight, elapsed / fadeDuration);
            yield return null;
        }

        volume.weight = targetWeight;

        if (targetWeight <= 0f)
        {
            volume.isGlobal = false;
        }

        _volumeFadeCoroutine = null;
    }

    #region Triggers (Transférés ici)
    private void OnTriggerEnter(Collider other)
    {
        if (linkedPuzzle != null && linkedPuzzle.IsAlreadyCompleted) return;
        
        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;

        TriggerVolumeFade(1f);

        if (linkedPuzzle != null)
        {
            linkedPuzzle.NotifyPlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (linkedPuzzle != null && linkedPuzzle.IsAlreadyCompleted) return;

        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;

        TriggerVolumeFade(0f);

        if (linkedPuzzle != null)
        {
            linkedPuzzle.NotifyPlayerExit();
        }
    }
    #endregion
}