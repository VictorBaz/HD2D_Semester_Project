using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderElementHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("Visual Feedback (DOTween)")]
    [SerializeField] private GameObject selectionVisual;
    [SerializeField] private float fadeDuration = 0.12f;
    
    public Action<float> OnValueChangedAction;
    
    private Slider _slider;
    private CanvasGroup _visualCanvasGroup;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        
        if (selectionVisual)
        {
            if (!selectionVisual.TryGetComponent(out _visualCanvasGroup))
            {
                _visualCanvasGroup = selectionVisual.AddComponent<CanvasGroup>();
            }
            _visualCanvasGroup.alpha = 0f;
        }
    }

    private void OnEnable()
    {
        if (_slider) _slider.onValueChanged.AddListener(HandleValueChanged);
    }

    private void OnDisable()
    {
        if (_slider) _slider.onValueChanged.RemoveListener(HandleValueChanged);
        if (_visualCanvasGroup) _visualCanvasGroup.alpha = 0f;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (_visualCanvasGroup)
        {
            _visualCanvasGroup.DOKill();
            _visualCanvasGroup.DOFade(1f, fadeDuration).SetUpdate(true);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (_visualCanvasGroup)
        {
            _visualCanvasGroup.DOKill();
            _visualCanvasGroup.DOFade(0f, fadeDuration).SetUpdate(true);
        }
    }

    private void HandleValueChanged(float value)
    {
        OnValueChangedAction?.Invoke(value);
    }
}