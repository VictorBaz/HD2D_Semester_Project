using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // AJOUT : Indispensable pour utiliser DOAnchorPosX

public class UIFresqueElement : MonoBehaviour
{
    [SerializeField] private CanvasGroup fresqueCanvasGroup;
    [SerializeField] private RectTransform rectTransformFresque;
    [SerializeField] private List<FresqueData> fresqueDatas;
    [SerializeField] private bool StopFresque;
    
    [Serializable]
    struct FresqueData
    {
        public int TargetX;
        public float DurationScroll;
        [Tooltip("Temps de pause sur cette partie de la fresque avant de passer à la suite")]
        public float HoldDuration;
    }

    private Tween _fresqueTween;
    

    private void Start() => FresqueLogic();

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
        GameplayEvents.TriggerPlayerEnable(false);
        
        yield return null;

        foreach (var data in fresqueDatas)
        {
            bool isMovementDone = false;

            _fresqueTween = rectTransformFresque.DOAnchorPosX(data.TargetX, data.DurationScroll)
                .SetEase(Ease.InOutCubic) 
                .SetUpdate(true)          
                .OnComplete(() => isMovementDone = true);

            yield return new WaitUntil(() => isMovementDone);

            if (data.HoldDuration > 0f) yield return new WaitForSecondsRealtime(data.HoldDuration);
        }
        
        fresqueCanvasGroup.DOFade(0f,1f).SetEase(Ease.InOutCubic);

        GameplayEvents.TriggerPlayerEnable(true);
    }

    private void OnDestroy() => _fresqueTween?.Kill();
}