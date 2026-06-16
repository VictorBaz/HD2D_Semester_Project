using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; 

public class UIFresqueElement : MonoBehaviour
{
    [System.Serializable]
    public struct FresqueData
    {
        public int TargetX;
        public float DurationScroll;
        [Tooltip("Temps de pause sur cette partie de la fresque avant de passer à la suite")]
        public float HoldDuration;
    }

    [System.Serializable]
    public struct FresqueSequence
    {
        public string SequenceID; 
        public GameObject FresqueVisual; 
        public List<FresqueData> Steps;
        public bool containAdeptes;
    }

    [Header("Configurations des Fresques")]
    [SerializeField] private List<FresqueSequence> fresqueSequences;

    [Header("UI References")]
    [SerializeField] private CanvasGroup fresqueCanvasGroup;
    [SerializeField] private RectTransform rectTransformFresque;
    [SerializeField] private RectTransform rectTransformTop;
    [SerializeField] private RectTransform rectTransformBot;
    [SerializeField] private RectTransform rectTransfVignette;
    [SerializeField] private RectTransform rectTransAdeptes;

    [SerializeField] private bool introOn;
    
    [Header("Vignette Noise Settings")]
    [SerializeField] private float vignetteNoiseSpeed = 0.1f; 
    private const float VIGNETTE_MIN_SCALE = 1f;
    private const float VIGNETTE_MAX_SCALE = 1.15f;

    private float originalYTop;
    private float originalYBot;
    private float originalYAdeptes;
    
    private Tween _fresqueTween;
    private Coroutine _fresqueCoroutine;
    private Coroutine _vignetteNoiseCoroutine; 
    private GameObject _activeVisual; 

    private void Awake()
    {
        if (fresqueCanvasGroup != null)
            fresqueCanvasGroup.alpha = 0f;

        foreach (var sequence in fresqueSequences)
        {
            if (sequence.FresqueVisual != null)
                sequence.FresqueVisual.SetActive(false);
        }
    }

    private void Start()
    {
        if (rectTransformTop != null) originalYTop = rectTransformTop.anchoredPosition.y;
        if (rectTransformBot != null) originalYBot = rectTransformBot.anchoredPosition.y;
        if (rectTransAdeptes != null) originalYAdeptes = rectTransAdeptes.anchoredPosition.y;

        if (introOn) PlayFresque("intro");
    }

    public void PlayFresque(string sequenceID)
    {
        if (string.IsNullOrEmpty(sequenceID)) return;

        FresqueSequence targetSequence = fresqueSequences.Find(f => f.SequenceID == sequenceID);

        if (targetSequence.Steps == null || targetSequence.Steps.Count == 0 || targetSequence.FresqueVisual == null)
        {
            Debug.LogWarning($"Fresque '{sequenceID}' introuvable, vide ou sans FresqueVisual assigné !");
            GameplayEvents.TriggerPlayerEnable(true);
            return;
        }

        StopCurrentFresque();

        _activeVisual = targetSequence.FresqueVisual;
        _activeVisual.SetActive(true);

        if (rectTransformFresque != null)
        {
            Vector2 localPos = rectTransformFresque.anchoredPosition;
            localPos.x = 0;
            rectTransformFresque.anchoredPosition = localPos;
        }

        _fresqueCoroutine = StartCoroutine(FresqueLogicIe(targetSequence.Steps,targetSequence.containAdeptes));
    }

    public void StopCurrentFresque()
    {
        if (_fresqueCoroutine != null) StopCoroutine(_fresqueCoroutine);
        
        _fresqueTween?.Kill();
        CleanVignetteNoise();
        AnimateCinematicBars(false, 0.5f);

        if (fresqueCanvasGroup != null)
        {
            fresqueCanvasGroup.DOFade(0f, 0.5f).SetEase(Ease.InOutCubic);
        }

        if (_activeVisual != null)
        {
            _activeVisual.SetActive(false);
            _activeVisual = null;
        }

        GameplayEvents.TriggerPlayerEnable(true);
    }

    private IEnumerator FresqueLogicIe(List<FresqueData> steps, bool adeptes)
    {
        yield return UiManager.Instance.FadeBlackScreen(1f, 0.5f);
        
        yield return fresqueCanvasGroup.DOFade(1f, 1f).SetEase(Ease.InOutCubic);
            
        GameplayEvents.TriggerPlayerEnable(false);
        yield return null;

        AnimateCinematicBars(true, 2.5f);

        if (adeptes)AddAdeptes(true, 2.5f);

        if (rectTransfVignette != null)
        {
            _vignetteNoiseCoroutine = StartCoroutine(VignetteNoiseRoutine());
        }

        int count = steps.Count;
        for (int i = 0; i < count; i++)
        {
            _fresqueTween = rectTransformFresque.DOAnchorPosX(steps[i].TargetX, steps[i].DurationScroll)
                .SetEase(Ease.InOutCubic) 
                .SetUpdate(true);          

            yield return _fresqueTween.WaitForCompletion();

            if (i == count - 1)
            {
                AnimateCinematicBars(false, 3f);
                if (adeptes)AddAdeptes(false, 3f);
            }
            
            if (steps[i].HoldDuration > 0f) 
            {
                yield return new WaitForSecondsRealtime(steps[i].HoldDuration);
            }
        }
        
        CleanVignetteNoise();

        yield return fresqueCanvasGroup.DOFade(0f, 1f).SetEase(Ease.InOutCubic);

        if (_activeVisual != null)
        {
            _activeVisual.SetActive(false);
            _activeVisual = null;
        }

        GameplayEvents.TriggerPlayerEnable(true);
        yield return UiManager.Instance.FadeBlackScreen(0f, 1.5f);
    }

    private IEnumerator VignetteNoiseRoutine()
    {
        float noiseSampleTime = UnityEngine.Random.Range(0f, 1000f);

        while (rectTransfVignette != null)
        {
            noiseSampleTime += Time.unscaledDeltaTime * vignetteNoiseSpeed;
            float noiseValue = Mathf.PerlinNoise(noiseSampleTime, 0f);
            float targetScale = Mathf.Lerp(VIGNETTE_MIN_SCALE, VIGNETTE_MAX_SCALE, noiseValue);

            rectTransfVignette.localScale = new Vector3(targetScale, targetScale, 1f);
            yield return null;
        }
    }

    private void CleanVignetteNoise()
    {
        if (_vignetteNoiseCoroutine != null)
        {
            StopCoroutine(_vignetteNoiseCoroutine);
            _vignetteNoiseCoroutine = null;
        }
        if (rectTransfVignette != null) 
        {
            rectTransfVignette.localScale = Vector3.one;
        }
    }

    private void AddAdeptes(bool show, float duration)
    {
        if (rectTransAdeptes != null)
        {
            float targetY = show ? Mathf.Abs(originalYAdeptes) : -Mathf.Abs(originalYAdeptes);
            rectTransAdeptes.DOAnchorPosY(targetY, duration).SetEase(Ease.InOutCubic).SetUpdate(true);
        }
    }

    private void AnimateCinematicBars(bool show, float duration)
    {
        if (rectTransformTop != null)
        {
            float targetY = show ? -Mathf.Abs(originalYTop) : Mathf.Abs(originalYTop);
            rectTransformTop.DOAnchorPosY(targetY, duration).SetEase(Ease.InOutCubic).SetUpdate(true);
        }

        if (rectTransformBot != null)
        {
            float targetY = show ? Mathf.Abs(originalYBot) : -Mathf.Abs(originalYBot);
            rectTransformBot.DOAnchorPosY(targetY, duration).SetEase(Ease.InOutCubic).SetUpdate(true);
        }
    }
    
    private void OnDestroy()
    {
        _fresqueTween?.Kill();
        CleanVignetteNoise();
        rectTransformTop?.DOKill();
        rectTransformBot?.DOKill();
        fresqueCanvasGroup?.DOKill();
    }
}