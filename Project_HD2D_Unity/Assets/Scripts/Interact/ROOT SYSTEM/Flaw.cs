using System;
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
    [SerializeField] private CanvasGroup feedbackCanvasGroup; 
    
    [Header("Blocking")]
    [SerializeField] private List<Parasite> blockers;

    [Header("Root Visuals")]
    [SerializeField] private Transform rootVisual;
    private bool _isCurrentlyTargeted;
    
    [Header("Energy Visuals")]
    public SpriteRenderer[] energyIcons;
    public Sprite fullEnergyIcon;
    public Sprite depletedEnergyIcon;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        if (feedbackCanvasGroup != null)
        {
            feedbackCanvasGroup.alpha = 0f;
            feedbackCanvasGroup.gameObject.SetActive(true); 
            
            feedbackCanvasGroup.interactable = false;
            feedbackCanvasGroup.blocksRaycasts = false;
        }

    }

    private void OnEnable()
    {
        foreach (var blocker in blockers)
        {
            blocker.OnDeath += UpdateRootVisuals;
        }
    }

    private void OnDisable()
    {
        foreach (var blocker in blockers)
        {
            blocker.OnDeath -= UpdateRootVisuals;
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
        if (feedbackCanvasGroup == null) return;

        Transform currentTarget = PlayerEvents.OnRequestCurrentLockTarget?.Invoke();
    
        bool isTargeted = currentTarget != null && currentTarget == GetLockTransform();

        bool finalTargetState = isTargeted && IsLockable();

        if (finalTargetState != _isCurrentlyTargeted)
        {
            _isCurrentlyTargeted = finalTargetState;
            AnimateFeedback(finalTargetState);
        }
    }

    private void AnimateFeedback(bool show)
    {
        feedbackCanvasGroup.DOKill();

        feedbackCanvasGroup.DOFade(show ? 1f : 0f, 0.25f)
            .SetEase(show ? Ease.OutCubic : Ease.InQuad)
            .SetUpdate(true); 

        if (show)
        {
            feedbackCanvasGroup.transform.DOKill();
            feedbackCanvasGroup.transform.localScale = Vector3.one * 0.7f;
            feedbackCanvasGroup.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
        }
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
        blockers.RemoveAll(p => p == null);
        return blockers.Count > 0;
    }
    
    #endregion

    #region Gizmos & Init
    private void OnDestroy()
    {
        feedbackCanvasGroup?.DOKill();
        feedbackCanvasGroup?.transform.DOKill();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle { normal = { textColor = Color.cyan }, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
        Handles.Label(transform.position + Vector3.up * 0.5f, "Flaw (Lockable)", style);
    }
#endif

    public void SetRoot(Root root) => this.root = root;
    #endregion
    
    public Vector3 GetPositionRootVisuals() => rootVisual.position;

    private void UpdateRootVisuals(Parasite parasite)
    {
        blockers.Remove(parasite);
        root.UpdateVisualEnergy();
    }

    public void SetIcons(int energy)
    {
        for (int i = 0; i < energyIcons.Length; i++)
        {
            if (energy >= i + 1) energyIcons[i].sprite = fullEnergyIcon;
            else energyIcons[i].sprite = depletedEnergyIcon;
        }
    }
}