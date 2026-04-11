using DG.Tweening;
using Enum;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPauseHandler : MonoBehaviour, 
    ISelectHandler, IDeselectHandler, IPointerClickHandler, ISubmitHandler
{
    [Header("Settings")]
    [SerializeField] private CanvasGroup canvasGroupSelectedButton;
    [SerializeField] private float fadeDuration = 0.15f;
    [SerializeField] private ButtonAction action;

    private void Awake()
    {
        if (canvasGroupSelectedButton != null)
            canvasGroupSelectedButton.alpha = 0;
    }

    public void OnSelect(BaseEventData eventData)
    {
        canvasGroupSelectedButton?.DOKill();
        canvasGroupSelectedButton?.DOFade(1f, fadeDuration).SetUpdate(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        canvasGroupSelectedButton?.DOKill();
        canvasGroupSelectedButton?.DOFade(0f, fadeDuration).SetUpdate(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ExecuteAction();
    }
    
    public void OnSubmit(BaseEventData eventData)
    {
        ExecuteAction();
    }

    private void ExecuteAction()
    {
        GameManager.Instance.ExecuteButtonAction(action);
        
        transform.DOPunchScale(Vector3.one * 0.1f, 0.2f).SetUpdate(true);
    }

    
}