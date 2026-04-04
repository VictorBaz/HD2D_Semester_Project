using UnityEngine;
using Manager;

public class Sap : MonoBehaviour, ISapLockable
{
    #region Variables
    [Header("Settings")]
    [SerializeField] private SpriteRenderer feedbackSprite;
    [SerializeField] private FeedbackLogic  feedbackLogic;

    private bool      _isEmpty;
    private Transform _playerTransform;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        HandleFeedbackDisplay();
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
    private void HandleFeedbackDisplay()
    {
        if (_isEmpty || feedbackSprite == null) return;

        float alpha = feedbackLogic.CalculateAlpha(transform.position);
        
        UpdateSpriteVisuals(alpha);

    }

    private void UpdateSpriteVisuals(float alpha)
    {
        Color c = feedbackSprite.color;
        c.a = alpha;
        feedbackSprite.color = c;
        
        feedbackSprite.enabled = alpha > 0.01f;
    }
    #endregion

    #region ISapLockable
    public Transform GetLockTransform() => transform;

    public bool IsLockable() => !_isEmpty;

    public float GetLockPriority() => 1f;

    public void GiveSap()
    {
        _isEmpty = true;
        
        if (feedbackSprite != null)
            feedbackSprite.enabled = false;
    }
    #endregion
}