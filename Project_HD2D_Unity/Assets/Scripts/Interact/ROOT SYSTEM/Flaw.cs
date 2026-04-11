using System.Collections.Generic;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Flaw : MonoBehaviour, IEnergyLockable, IRootLink
{
    #region Variables
    private Root root;
    
    [Header("References")]
    [SerializeField] private Transform pivotPoint;
    [SerializeField] private SpriteRenderer feedbackSprite;
    
    [Header("Blocking")]
    [SerializeField] private List<Parasite> blockers;

    private bool _isCurrentlyTargeted; 
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        if (feedbackSprite != null)
        {
            Color c = feedbackSprite.color;
            c.a = 0f;
            feedbackSprite.color = c;
            feedbackSprite.enabled = false;
        }
    }

    private void Update()
    {
        HandleFeedback();
    }
    #endregion

    #region Feedback Logic
    private void HandleFeedback()
    {
        if (feedbackSprite == null) return;

        bool isTargeted = false;
        if (PlayerEvents.OnRequestCurrentLockTarget != null)
        {
            isTargeted = PlayerEvents.OnRequestCurrentLockTarget.Invoke() == pivotPoint;
        }

        bool finalTargetState = isTargeted && IsLockable();

        if (finalTargetState != _isCurrentlyTargeted)
        {
            _isCurrentlyTargeted = finalTargetState;
            AnimateSprite(finalTargetState);
        }
    }

    private void AnimateSprite(bool show)
    {
        feedbackSprite.DOKill();

        if (show) feedbackSprite.enabled = true;

        feedbackSprite.DOFade(show ? 1f : 0f, 0.2f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => {
                if (!show) feedbackSprite.enabled = false;
            });
    }
    #endregion
    
    #region IEnergyLockable
    public Transform GetLockTransform() => pivotPoint;
    public bool IsLockable() => root != null && !IsBlocked();
    public float GetLockPriority() => 1f;
    public bool IsContainingEnergy() => root != null && root.IsContainingEnergy();
    public bool IsAtMaximumEnergy() => root != null && root.IsAtMaximumEnergy();
    public void AddEnergy() => root?.AddEnergy();
    public void RemoveEnergy() => root?.RemoveEnergy();
    
    public bool IsBlocked()
    {
        if (blockers == null) return false;
        foreach (var p in blockers) if (p != null) return true;
        return false;
    }
    #endregion

    #region Gizmos & Init
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle { normal = { textColor = Color.white }, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
        Handles.Label(transform.position, "Flaw", style);
    }
#endif

    public void SetRoot(Root root) => this.root = root;
    #endregion
}