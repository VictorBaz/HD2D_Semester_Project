using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; 

public class UIFresqueElement : MonoBehaviour
{
    [SerializeField] private CanvasGroup fresqueCanvasGroup;
    [SerializeField] private RectTransform rectTransformFresque;
    [SerializeField] private RectTransform rectTransformTop;
    [SerializeField] private RectTransform rectTransformBot;
    [SerializeField] private List<FresqueData> fresqueDatas;
    [SerializeField] private bool StopFresque;

    private float yPositionTop;
    private float yPositionBot;
    
    [Serializable]
    struct FresqueData
    {
        public int TargetX;
        public float DurationScroll;
        [Tooltip("Temps de pause sur cette partie de la fresque avant de passer à la suite")]
        public float HoldDuration;
    }

    private Tween _fresqueTween;

    private void Awake()
    {
        fresqueCanvasGroup.alpha = 0f;
    }

    private void Start()
    {
        FresqueLogic();
        yPositionBot =  rectTransformBot.anchoredPosition.y;
        yPositionTop =  rectTransformTop.anchoredPosition.y;
    }

    private void FresqueLogic()
    {
        if (rectTransformFresque == null || fresqueDatas == null || fresqueDatas.Count == 0) 
        {
            GameplayEvents.TriggerPlayerEnable(true);
            return;
        }

        if (StopFresque && fresqueCanvasGroup != null)
        {
            fresqueCanvasGroup.DOFade(0f,1f).SetEase(Ease.InOutCubic);
            GameplayEvents.TriggerPlayerEnable(true);
            return;
        }

        StartCoroutine(FresqueLogicIe());
    }

    private IEnumerator FresqueLogicIe()
    {
        yield return UiManager.Instance.FadeBlackScreen(1f, 0.5f);
        
        yield return fresqueCanvasGroup.DOFade(1f,1f).SetEase(Ease.InOutCubic);
            
        GameplayEvents.TriggerPlayerEnable(false);
        
        yield return null;

        CinematicEffect();

        int count = fresqueDatas.Count;
        
        for (int i = 0; i < count; i++)
        {
            bool isMovementDone = false;

            _fresqueTween = rectTransformFresque.DOAnchorPosX(fresqueDatas[i].TargetX, fresqueDatas[i].DurationScroll)
                .SetEase(Ease.InOutCubic) 
                .SetUpdate(true)          
                .OnComplete(() => isMovementDone = true);

            yield return new WaitUntil(() => isMovementDone);

            if (i == count - 1)
            {
                CinematicEffect(3f);
                
            }
            
            if (fresqueDatas[i].HoldDuration > 0f) yield return new WaitForSecondsRealtime(fresqueDatas[i].HoldDuration);
        }
        
        yield return fresqueCanvasGroup.DOFade(0f,1f).SetEase(Ease.InOutCubic);

        GameplayEvents.TriggerPlayerEnable(true);
        
        yield return UiManager.Instance.FadeBlackScreen(0f, 1.5f);
    }

    private void CinematicEffect(float duration = 2.5f)
    {
        rectTransformTop.DOAnchorPosY(-yPositionTop, duration).SetEase(Ease.InOutCubic);
        yPositionTop = -yPositionTop;
        rectTransformBot.DOAnchorPosY(-yPositionBot, duration).SetEase(Ease.InOutCubic);
        yPositionBot = -yPositionBot;
    }
    
    private void OnDestroy() => _fresqueTween?.Kill();
}