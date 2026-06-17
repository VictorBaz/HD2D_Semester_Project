using System;
using System.Collections;
using Interface;
using Player.State;
using Script.Manager;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerActionHandler))]
public class PlayerManager : MonoBehaviour, IDamageable, IDataPersistence
{
    #region Variables

    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerAnimationManager animationManager;
    [SerializeField] private InputManager          inputManager;
    [SerializeField] private LockOnSystem          lockOnSystem;
    [SerializeField] private VfxManagerPlayer      vfxManagerPlayer;
    [SerializeField] private Transform             cameraTransform;
    [SerializeField] private Transform             playerHead;
    [SerializeField] private Rigidbody             rb;
    [SerializeField] private CapsuleCollider       playerCollider;
    [SerializeField] private PlayerData            playerDataRaw;
    [SerializeField] private GameObject            previewElement;
    [SerializeField] private LineRenderer          trajectoryLineRenderer;
    [SerializeField] private Transform             trajectoryStartTransform;

    public PlayerBaseState     CurrentPlayerState { get; private set; }
    public PlayerLocomotionState LocomotionState  { get; private set; }
    public PlayerAttackState   AttackState        { get; private set; }
    public PlayerLandingState  LandingState       { get; private set; }
    public PlayerDashState     DashState          { get; private set; }
    public PlayerCarryState    CarryState         { get; private set; }
    public PlayerHitState      HitState           { get; private set; }
    public PlayerParryState    ParryState         { get; private set; }
    public PlayerJumpState     JumpState          { get; private set; }
    public PlayerFallState     FallState          { get; private set; }
    public PlayerBumpState     BumpState          { get; private set; }

    public PlayerStateContext Context { get; private set; }

    private PlayerDataInstance playerData;

    private Vector3   originPos;
    private Vector3   checkPointPos;
    private Coroutine respawnRoutine;

    [Header("Passive Regen")]
    [SerializeField] private float regenDelay     = 5f;
    [SerializeField] private float regenSpeed     = 2f;

    private float timeSinceLastDamage = Mathf.Infinity;
    private float regenAccumulator = 0f;
    
    private PreviewEjectionPlayer  previewEjectionPlayer;

    private bool enable = true;

    private bool Enable
    {
        get => enable;
        set
        {
            enable = value;
            playerController.Rb.isKinematic = !value;
        }
    }

    public bool IsPlayerRespawning { get; private set; } = false;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        InitStates();
        
        playerData = playerDataRaw.Init();

        previewEjectionPlayer = new PreviewEjectionPlayer(
            previewElement,
            trajectoryLineRenderer,
            trajectoryStartTransform,
            playerData.GroundMask
        );
        
        Context = new PlayerStateContext
        {
            Controller          = playerController,
            AnimationManager    = animationManager,
            LockOnSystem        = lockOnSystem,
            InputManager        = inputManager,
            Rb                  = rb,
            CameraTransform     = cameraTransform,
            PlayerTransform     = transform,
            StateMachine        = this,
            PlayerData          = playerData,
            VfxManagerPlayer    = vfxManagerPlayer,
            PlayerHeadTransform = playerHead,
            Collider            = playerCollider,
            PreviewEjectionPlayer = previewEjectionPlayer,
        };

        TransitionTo(LocomotionState);

        lockOnSystem.InitData(playerData);
        lockOnSystem.InitManager(Context);
        playerController.InitData(playerData);

        PlayerEvents.OnRequestPlayerTransform = GetTransform;
        PlayerEvents.OnRequestPlayerContext   = GetContext;

        originPos = transform.position;

        GameplayEvents.OnCredits += CreditsState;
    }

    private void Start()
    {
        UiEvents.TriggerEnergyChanged(Context.PlayerData.Energy, Context.PlayerData.MaxEnergy);
        UiEvents.TriggerSapChanged(Context.PlayerData.Sap);
        if (UiManager.Instance) UiManager.Instance.SetupLifeUi(Context.PlayerData.MaxLife, Context.PlayerData.MaxLife);
    }

    private void Update()
    {
        if(!Enable) return;
        CurrentPlayerState.UpdateState(Context);
        HandlePassiveRegen();
    }

    private void FixedUpdate()
    {
        if(!Enable) return;
        CurrentPlayerState.FixedUpdateState(Context);
    }

    private void OnDestroy()
    {
        PlayerEvents.OnRequestPlayerTransform   = null;
        PlayerEvents.OnRequestPlayerContext     = null;
        PlayerEvents.OnRequestCurrentLockTarget = null;
        GameplayEvents.OnCredits -= CreditsState;
    }

    private void OnEnable()
    { 
        GameplayEvents.OnCheckpoint += UpdateCheckPoint;
        GameplayEvents.OnPlayerBlocked += TogglePlayer;
    }

    private void OnDisable()
    {
        GameplayEvents.OnCheckpoint -= UpdateCheckPoint;
        GameplayEvents.OnPlayerBlocked -= TogglePlayer;
    }
    

    #endregion

    #region State Machine

    private void InitStates()
    {
        LocomotionState = new PlayerLocomotionState();
        AttackState     = new PlayerAttackState();
        LandingState    = new PlayerLandingState();
        DashState       = new PlayerDashState();
        CarryState      = new PlayerCarryState();
        HitState        = new PlayerHitState();
        ParryState      = new PlayerParryState();
        JumpState       = new PlayerJumpState();
        FallState       = new PlayerFallState();
        BumpState       = new PlayerBumpState();
    }

    public void TransitionTo(PlayerBaseState newState)
    {
        if(!Enable) return;
        
        CurrentPlayerState?.ExitState(Context);
        CurrentPlayerState = newState;
        CurrentPlayerState.EnterState(Context);
    }

    #endregion

    #region IDamageable

    public void TakeDamage(int value, Vector3 hitDirection)
    {
        if (!CurrentPlayerState.CanTakeDamage)     return;
        if (CurrentPlayerState.IsParryWindowActive) return;

        timeSinceLastDamage = 0f;
        regenAccumulator    = 0f;
        
        Context.HitDirection    = hitDirection;
        Context.PlayerData.Life -= value;

        UiManager.Instance?.UpdateLifeUi(Context.PlayerData.Life);

        if (Context.PlayerData.Life <= 0)
        {
            TriggerRespawn(true);
            return;
        }

        TransitionTo(HitState);
    }

    public Transform GetTransform() => transform;

    public bool IsInParryWindow()
        => CurrentPlayerState is PlayerParryState p && p.IsParryWindowActive;

    public bool IsInParryWindowPerfect()
        => CurrentPlayerState is PlayerParryState p && p.IsPerfectWindowActive;

    #endregion

    #region Passive Regen

    private void HandlePassiveRegen()
    {
        if (Context.PlayerData.IsPlayerFullLife()) return;

        timeSinceLastDamage += Time.deltaTime;
        if (timeSinceLastDamage < regenDelay) return;

        regenAccumulator += regenSpeed * Time.deltaTime;

        if (regenAccumulator < 1f) return;

        int points = Mathf.FloorToInt(regenAccumulator);
        regenAccumulator -= points;

        Context.PlayerData.Life = Mathf.Min(Context.PlayerData.Life + points, Context.PlayerData.MaxLife);
        UiManager.Instance?.UpdateLifeUi(Context.PlayerData.Life);
    }

    #endregion

    #region Save System

    public void LoadData(GameData data)
    {
        if (data.PlayerData != null)
        {
            playerData.Life   = data.PlayerData.Life;
            playerData.Energy = data.PlayerData.Energy;
            playerData.Sap    = data.PlayerData.Sap;
        }

        if (string.IsNullOrEmpty(data.LastVisitedPuzzleId)) return;

        Puzzle lastPuzzle = PuzzleManager.Instance.GetPuzzleById(data.LastVisitedPuzzleId);

        if (lastPuzzle != null)
        {
            transform.position = lastPuzzle.SpawnPoint.position;
            Physics.SyncTransforms();
        }
    }

    public void SaveData(ref GameData data)
    {
        data.PlayerData = new PlayerSaveData(playerData, checkPointPos);
    }

    #endregion

    private PlayerStateContext GetContext() => Context;

    private IEnumerator PlayerRespawn(bool isDead)
    {
        IsPlayerRespawning = true;
        
        playerController.enabled = false;

        if (!isDead) vfxManagerPlayer.TriggerSplash();

        yield return StartCoroutine(UiManager.Instance.FadeBlackScreen(1f, 0.5f));

        if (isDead)
        {
            playerData         = playerDataRaw.Init();
            Context.PlayerData = playerData;

            if (UiManager.Instance)
                UiManager.Instance.SetupLifeUi(Context.PlayerData.MaxLife, Context.PlayerData.MaxLife);

            UiEvents.TriggerEnergyChanged(Context.PlayerData.Energy, Context.PlayerData.MaxEnergy);
            UiEvents.TriggerSapChanged(Context.PlayerData.Sap);

            var managerData = DataPersistenceManager.Instance;

            if (managerData && managerData.HasGameData() && managerData.CanTPPlayerToLastPos())
            {
                var lastPuzzle = PuzzleManager.Instance.GetPuzzleById(managerData.GetLastVisitedPuzzleId());

                transform.position = lastPuzzle && lastPuzzle.SpawnPoint
                    ? lastPuzzle.SpawnPoint.position
                    : originPos;
            }
            else
            {
                transform.position = checkPointPos != Vector3.zero ? checkPointPos : originPos;
            }
        }
        else
        {
            transform.position = checkPointPos != Vector3.zero ? checkPointPos : originPos;
        }

        timeSinceLastDamage = 0f;
        regenAccumulator    = 0f;

        rb.linearVelocity = Vector3.zero;
        
        Physics.SyncTransforms();

        rb.linearVelocity = Vector3.zero;
        
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(UiManager.Instance.FadeBlackScreen(0f, isDead ? 0.7f : 0.1f));

        playerController.enabled = true;
        
        IsPlayerRespawning = false;
    }

    public void TriggerRespawn(bool isDead)
    {
        if (respawnRoutine != null) StopCoroutine(respawnRoutine);
        respawnRoutine = StartCoroutine(PlayerRespawn(isDead));
    }

    private void UpdateCheckPoint(Vector3 pos) => checkPointPos = pos;
    

    #region Helper

    public void TogglePlayer(bool on)
    {
        Enable = on;

        if (on)
        {
            TransitionTo(LocomotionState);
        }
    }   

    public InputManager GetInputManager() => inputManager;

    public void CreditsState(float time)
    {
        TogglePlayer(true);
    }

    #endregion
}