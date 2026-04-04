using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Manager;

public class Flaw : MonoBehaviour, IEnergyLockable, IRootLink
{
    #region Variables
    private Root root;
    
    [Header("References")]
    [SerializeField] private Transform pivotPoint;
    [SerializeField] private SpriteRenderer feedbackSprite;
    
    [Header("Settings")]
    [SerializeField] private FeedbackLogic feedbackLogic;

    private Transform _playerTransform;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        HandleFeedback();
    }
    #endregion

    #region Initialization
    private void Initialize()
    {
        if (EventManager.OnRequestPlayerTransform != null)
        {
            _playerTransform = EventManager.OnRequestPlayerTransform.Invoke();
            feedbackLogic.Initialize(_playerTransform);
        }
    }
    #endregion

    #region Feedback Logic
    private void HandleFeedback()
    {
        if (feedbackSprite == null) return;

        if (!IsLockable())
        {
            feedbackSprite.enabled = false;
            return;
        }

        float alpha = feedbackLogic.CalculateAlpha(transform.position);
        UpdateVisuals(alpha);
        
    }

    private void UpdateVisuals(float alpha)
    {
        Color c = feedbackSprite.color;
        c.a = alpha;
        feedbackSprite.color = c;
        feedbackSprite.enabled = alpha > 0.01f;
    }
    #endregion
    
    #region IEnergyLockable
    public Transform GetLockTransform() => pivotPoint;
    
    public bool IsLockable() => root != null; 
    
    public float GetLockPriority() => 1f;

    public bool IsContainingEnergy() => root != null && root.IsContainingEnergy();
    public bool IsAtMaximumEnergy() => root != null && root.IsAtMaximumEnergy();

    public void AddEnergy() => root?.AddEnergy();
    public void RemoveEnergy() => root?.RemoveEnergy();
    #endregion

    #region Gizmos
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle
        {
            normal = { textColor = Color.white },
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };

        Handles.Label(transform.position, "Flaw", style);
    }
#endif
    #endregion

    #region Init
    public void SetRoot(Root root)
    {
        this.root = root;
    }
    #endregion
}