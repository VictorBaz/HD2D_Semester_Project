using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // AJOUT : Indispensable pour utiliser DOAnchorPosX

public class UIFresqueElement : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransformFresque;
    [SerializeField] private List<FresqueData> fresqueDatas;
    
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
        GameplayEvents.TriggerPlayerBlocked(true);
    }

    private void Start() => FresqueLogic();

    private void FresqueLogic()
    {
        if (rectTransformFresque == null || fresqueDatas == null || fresqueDatas.Count == 0)
        {
            GameplayEvents.TriggerPlayerBlocked(false);
            return;
        }

        StartCoroutine(FresqueLogicIe());
    }

    private IEnumerator FresqueLogicIe()
    {
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

        GameplayEvents.TriggerPlayerBlocked(false);
    }

    private void OnDestroy() => _fresqueTween?.Kill();
}