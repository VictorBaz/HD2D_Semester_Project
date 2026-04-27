using System;
using System.Collections.Generic; 
using UnityEngine;

public class Parasite : MonoBehaviour, IDamageable, IDataPersistence
{
    
    #region Events
    public event Action<Parasite> OnDeath; 
    #endregion

    #region Variables
    [Header("Stats")]
    [SerializeField] private int life = 3;
    [SerializeField] private int lifeMax = 3;

    [SerializeField] private EntityID entityID;
    
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
        if (PlayerEvents.OnRequestPlayerContext != null)
            _playerContext = PlayerEvents.OnRequestPlayerContext.Invoke();
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
        UiEvents.TriggerSapChanged(_playerContext.PlayerData.Sap);
        life--;
        if (life <= 0) Die();
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }
    #endregion

    #region Save

    public void LoadData(GameData data)
    {
        ParasiteSaveData myData = data.parasiteDataList.Find(x => x.id == entityID.ID);
        if (myData != null)
        {
            this.life = myData.currentLife;
            this._isDead = myData.isDead;

            if (_isDead)
            {
                gameObject.SetActive(false); 
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        int index = data.parasiteDataList.FindIndex(x => x.id == entityID.ID);
        if (index != -1)
        {
            data.parasiteDataList[index].currentLife = this.life;
            data.parasiteDataList[index].isDead = this._isDead;
        }
        else
        {
            data.parasiteDataList.Add(new ParasiteSaveData { 
                id = entityID.ID, 
                currentLife = this.life, 
                isDead = this._isDead 
            });
        }
    }

    #endregion
}