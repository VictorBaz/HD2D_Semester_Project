using System;
using System.Collections;
using System.Collections.Generic;
using Script.Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class Parasite : MonoBehaviour, IDamageable, IDataPersistence
{
    
    #region Events
    public event Action<Parasite> OnDeath; 
    #endregion

    #region Variables
    [Header("Stats")]
    [SerializeField] private int life = 3;
    [SerializeField] private int lifeMax = 3;
    [SerializeField] private bool isBoss = false; 

    [SerializeField] private EntityID entityID;
    
    private PlayerStateContext playerContext;
    private bool isDead;
    
    [Header("Achievement")]
    private SteamAchivementManager ACHManager;
    private string normalAchievementId = "ACH_KILLED_PARASITE";
    [SerializeField] private string BossAchievementId = "ACH_LVL_0";
    
    [Header("Animation")]
    [SerializeField] private Animator animatorParasite;
    [SerializeField] private AnimationClip animationClipDeath;
    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int DeathHash = Animator.StringToHash("Death");
    private Coroutine deathCoroutine;
    
    [Header("VFX")]
    [SerializeField] private SkinnedMeshRenderer skinnedRenderer;
    [SerializeField, Range(0.1f, 2f)] private float deathAnimationSpeed = 0.5f;
    [SerializeField] private  ParticleSystem vfxHit;
    [SerializeField] private  ParticleSystem vfxParasiteAndLock;
    private MaterialPropertyBlock _propBlock;
    private static readonly int DissolveHash = Shader.PropertyToID("_Progression");

    [Header("Boss UI Direct Link")] 
    [SerializeField] private Slider bossLifeSlider;
    
    [Header("Events")]
    [SerializeField] private UnityEvent onDeath;
    
    #endregion

    #region Unity Lifecycle
    
    private void Start()
    {
        Init();
    }
    #endregion

    #region Initialization
    private void Init()
    {
        ACHManager = SteamAchivementManager.Instance;
        
        if (ACHManager == null)
        {
            Debug.LogWarning("Steam manager is NULL !");
        }
        
        life = lifeMax;
        
        if (PlayerEvents.OnRequestPlayerContext != null)
            playerContext = PlayerEvents.OnRequestPlayerContext.Invoke();
        
        _propBlock = new MaterialPropertyBlock();

        if (bossLifeSlider != null)
        {
            bossLifeSlider.maxValue = lifeMax;
            bossLifeSlider.value = life;
            bossLifeSlider.gameObject.SetActive(true);
        }
    }
    #endregion

    #region IDamageable Implementation
    public void TakeDamage(int value, Vector3 hitDirection)
    {
        if (isBoss)
            if (playerContext.PlayerData.Sap < life)
            {
                animatorParasite.SetTrigger(HitHash);
                vfxHit.TriggerParticleSystem();
                if (SoundManager.Instance) SoundManager.Instance.PlaySfx(SoundType.Damage_Ineffective);
                return;
            }
        
        if (isDead || playerContext == null) return;
        
        if (playerContext.PlayerData.IsSapEmpty())
        {
            if (SoundManager.Instance) SoundManager.Instance.PlaySfx(SoundType.Damage_Ineffective);
            return;
        }
        
        ApplyDamage();
    }

    public Transform GetTransform() => transform;
    public bool IsInParryWindow() => false;
    public bool IsInParryWindowPerfect() => false;
    #endregion

    #region Combat Logic
    private void ApplyDamage()
    {
        playerContext.PlayerData.RemoveSap();
        UiEvents.TriggerSapChanged(playerContext.PlayerData.Sap);
        
        life--;
        
        if (bossLifeSlider)
        {
            bossLifeSlider.value = life;
        }
        
        if (SoundManager.Instance) SoundManager.Instance.PlaySfx(SoundType.Damage_Effective);
        
        if (life <= 0)
        {
            if (bossLifeSlider != null) bossLifeSlider.gameObject.SetActive(false);
            ACH_Unlock();
            Die();
            onDeath.Invoke();
        }
        else
        {
            animatorParasite.SetTrigger(HitHash);
        }
    }

    private void Die()
    {
        if (isDead) return;
    
        animatorParasite.speed = deathAnimationSpeed; 
    
        animatorParasite.SetTrigger(DeathHash);
        isDead = true;
        
        OnDeath?.Invoke(this);
        
    
        GamepadVibrationHelper.Vibrate(0.25f,1f,0.10f);
        if (deathCoroutine != null) StopCoroutine(deathCoroutine);
        deathCoroutine = StartCoroutine(DeathIe());
    }

    private IEnumerator DeathIe()
    {
        float realDuration = animationClipDeath.length / deathAnimationSpeed;
        float elapsed = 0;
        
        while (elapsed < realDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / realDuration);
            SetDissolve(progress);
            yield return null;
        }
        
        Destroy(gameObject);
    }
    
    private void SetDissolve(float value)
    {
        _propBlock ??= new MaterialPropertyBlock();
        skinnedRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat(DissolveHash, value);
        skinnedRenderer.SetPropertyBlock(_propBlock);
    }
    
    void ACH_Unlock()
    {
        if (ACHManager == null)
            return;
        if (isBoss)
        {
            if (!ACHManager.IsThisAchievementUnlocked(BossAchievementId))
            {
                ACHManager.UnlockAchivement(BossAchievementId);
            }
        }
        else
        {
            if (!ACHManager.IsThisAchievementUnlocked(normalAchievementId))
            {
                ACHManager.UnlockAchivement(normalAchievementId);
            }
        }
        
    }
    
    #endregion

    #region Save

    public void LoadData(GameData data)
    {
        ParasiteSaveData myData = data.parasiteDataList.Find(x => x.id == entityID.ID);
        if (myData != null)
        {
            this.life = myData.currentLife;
            this.isDead = myData.isDead;

            if (isDead)
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
            data.parasiteDataList[index].isDead = this.isDead;
        }
        else
        {
            data.parasiteDataList.Add(new ParasiteSaveData { 
                id = entityID.ID, 
                currentLife = this.life, 
                isDead = this.isDead 
            });
        }
    }

    #endregion

    #region VFX

    public void EmitParasitePresenceVfx()
    {
        vfxParasiteAndLock.TriggerParticleSystem();
    }

    #endregion
}