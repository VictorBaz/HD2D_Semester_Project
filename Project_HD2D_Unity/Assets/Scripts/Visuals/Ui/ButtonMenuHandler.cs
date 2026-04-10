using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; 

public class ButtonMenuHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Visual Feedback")]
    [SerializeField] private GameObject selectionVisual;
    [SerializeField] private bool disableOnExit = true;

    [Header("DOTween Settings")]
    [SerializeField] private float scaleDuration = 0.1f;
    [SerializeField] private Ease hoverEase = Ease.OutQuad;

    private Vector3 originalScale;
    private CanvasGroup visualCanvasGroup;

    private void Awake()
    {
        originalScale = transform.localScale;

        if (selectionVisual != null)
        {
            if (!selectionVisual.TryGetComponent(out visualCanvasGroup))
                visualCanvasGroup = selectionVisual.AddComponent<CanvasGroup>();
            
            visualCanvasGroup.alpha = 0;
            selectionVisual.SetActive(true); 
        }
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(originalScale * 1.05f, scaleDuration).SetEase(hoverEase);

        if (visualCanvasGroup != null)
            visualCanvasGroup.DOFade(1f, scaleDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(originalScale, scaleDuration).SetEase(hoverEase);

        if (disableOnExit && visualCanvasGroup != null)
            visualCanvasGroup.DOFade(0f, scaleDuration);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(originalScale * 0.92f, scaleDuration).SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(originalScale * 1.05f, scaleDuration).SetEase(Ease.OutBack);
    }

    private void OnDestroy()
    {
        transform.DOKill();
        if (visualCanvasGroup != null) visualCanvasGroup.DOKill();
    }
}