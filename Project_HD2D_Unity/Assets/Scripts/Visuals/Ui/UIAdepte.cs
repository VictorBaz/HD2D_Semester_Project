using UnityEngine;
using DG.Tweening;

public class UIAdepte : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private RectTransform targetRectTransform;
    [SerializeField] private float maxAngle = 15f;     
    [SerializeField] private float duration = 1.2f;    
    [SerializeField] private Ease easeType = Ease.InOutQuad; 

    private Tween _rotationTween;

    private void Start()
    {
        if (targetRectTransform == null)
        {
            targetRectTransform = GetComponent<RectTransform>();
        }

        StartRotation();
    }

    public void StartRotation()
    {
        _rotationTween?.Kill();

        if (targetRectTransform == null) return;

        targetRectTransform.localRotation = Quaternion.Euler(0f, 0f, -maxAngle);

        _rotationTween = targetRectTransform.DOLocalRotate(new Vector3(0f, 0f, maxAngle), duration)
            .SetEase(easeType)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true); 
    }

    public void StopRotation()
    {
        _rotationTween?.Kill();
    }

    private void OnDestroy()
    {
        _rotationTween?.Kill();
    }
}