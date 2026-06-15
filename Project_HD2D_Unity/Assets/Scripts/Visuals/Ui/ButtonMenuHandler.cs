using DG.Tweening;
using Enum;
using Script.Manager;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonMenuHandler : MonoBehaviour, 
    ISelectHandler, IDeselectHandler, IPointerClickHandler, ISubmitHandler
{
    [Header("Visual Feedback")]
    [SerializeField] private GameObject selectionVisual;
    [SerializeField] private float scaleDuration = 0.1f;
    [SerializeField] private ButtonAction action;
    
    private Vector3 originalScale;
    private CanvasGroup visualCanvasGroup;

    private void Awake()
    {
        originalScale = transform.localScale;
        if (selectionVisual != null && !selectionVisual.TryGetComponent(out visualCanvasGroup))
            visualCanvasGroup = selectionVisual.AddComponent<CanvasGroup>();
        
        if (visualCanvasGroup != null) visualCanvasGroup.alpha = 0;
    }

    public void OnSelect(BaseEventData eventData)
    {
        transform.DOScale(originalScale * 1.05f, scaleDuration).SetEase(Ease.OutQuad);
        visualCanvasGroup?.DOFade(1f, scaleDuration);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.DOScale(originalScale, scaleDuration).SetEase(Ease.OutQuad);
        visualCanvasGroup?.DOFade(0f, scaleDuration);
        if (SoundManager.Instance)SoundManager.Instance.PlaySfx(SoundType.UI_Switch);
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
        if (SoundManager.Instance)SoundManager.Instance.PlaySfx(SoundType.UI_Select);
        transform.DOKill();
        transform.DOPunchScale(Vector3.one * 0.1f, 0.2f).SetUpdate(true);
    }
}