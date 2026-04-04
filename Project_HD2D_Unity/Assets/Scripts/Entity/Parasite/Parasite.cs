using System;
using System.Collections.Generic; 
using UnityEngine;

public class Parasite : MonoBehaviour, IDamageable
{
    
    #region Events
    public event Action OnDeath; 
    #endregion

    #region Variables
    [Header("Stats")]
    [SerializeField] private int life = 3;
    [SerializeField] private int lifeMax = 3;

    [Header("Blocking")]
    [SerializeField] private List<VATManager> blockedVats;

    private PlayerStateContext _playerContext;
    private bool _isDead;
    #endregion

    #region Unity Lifecycle
    private void Start() => Init();
    #endregion

    #region Initialization
    private void Init()
    {
        life = lifeMax;
        if (EventManager.OnRequestPlayerContext != null)
            _playerContext = EventManager.OnRequestPlayerContext.Invoke();
    }
    #endregion

    #region IDamageable Implementation
    public void TakeDamage(int value, Vector3 hitDirection)
    {
        if (_isDead || _playerContext == null) return;
        if (_playerContext.PlayerData.IsSapEmpty()) return;
        ApplyDamage();
    }

    public Transform GetTransform() => transform;
    public bool IsInParryWindow() => false;
    public bool IsInParryWindowPerfect() => false;
    #endregion

    #region Combat Logic
    private void ApplyDamage()
    {
        _playerContext.PlayerData.RemoveSap();
        life--;
        if (life <= 0) Die();
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
    #endregion
}