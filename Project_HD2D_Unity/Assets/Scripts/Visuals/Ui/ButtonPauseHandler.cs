using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPauseHandler : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler, 
    ISelectHandler, IDeselectHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")]
    [SerializeField] private CanvasGroup canvasGroupSelectedButton;
    [SerializeField] private float fadeDuration = 0.15f;
    [SerializeField] private float punchScaleAmount = 0.95f; 

    private void Awake()
    {
        if (canvasGroupSelectedButton != null)
            canvasGroupSelectedButton.alpha = 0;
    }

    private void ToggleHighlight(bool show)
    {
        if (canvasGroupSelectedButton == null) return;

        canvasGroupSelectedButton.DOKill();
        canvasGroupSelectedButton.DOFade(show ? 1f : 0f, fadeDuration).SetUpdate(true); 
    
    }

    #region Hover & Selection
    public void OnPointerEnter(PointerEventData eventData) => ToggleHighlight(true);
    public void OnPointerExit(PointerEventData eventData) => ToggleHighlight(false);

    public void OnSelect(BaseEventData eventData) => ToggleHighlight(true);
    public void OnDeselect(BaseEventData eventData) => ToggleHighlight(false);
    #endregion

    #region Click Feedback
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(punchScaleAmount, 0.1f).SetUpdate(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(1f, 0.1f).SetUpdate(true);
    }
    #endregion

    private void OnDestroy()
    {
        canvasGroupSelectedButton?.DOKill();
        transform.DOKill();
    }
}