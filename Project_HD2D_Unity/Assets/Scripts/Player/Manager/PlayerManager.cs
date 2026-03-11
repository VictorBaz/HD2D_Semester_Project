using System;
using System.Diagnostics;
using System.Timers;
using Player.State;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private InputManager     inputManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private AnimationManager animationManager;
    [SerializeField] private LockOnSystem     lockOnSystem;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform playerHead;

    [SerializeField] private PlayerCursor   playerCursor;
    [SerializeField] private Rigidbody      rb;
    [SerializeField] private ShootingSystem shootingSystem;
    [SerializeField] private PlayerData playerDataRaw;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private VfxManager vfxManager;

    public PlayerBaseState CurrentPlayerState { get; private set; }
    
    public PlayerLocomotionState  LocomotionState  { get; private set; }
    public PlayerAirState         AirState         { get; private set; }
    public PlayerAttackMeleeState MeleeAttackState { get; private set; }
    public PlayerLandingState     LandingState     { get; private set; }
    public PlayerDashState        DashState        { get; private set; }

    private PlayerStateContext context;
    
    private PlayerDataInstance playerData;
    
    
    //TEMPORARY

    private float dashCooldown     = 0.2f;
    private float dashCooldownTimer = 0f;
    
    [SerializeField] private int mana;
    [SerializeField] private UiManager uiManager;

    public int Mana
    {
        get => mana;
        
        set
        {
            value = Mathf.Clamp(value, 0, 9);
            mana = value;
            uiManager.UpdateEnergyTxt(value);
        }
    }

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        
        LocomotionState  = new PlayerLocomotionState();
        AirState         = new PlayerAirState();
        MeleeAttackState = new PlayerAttackMeleeState();
        LandingState     = new PlayerLandingState();
        DashState        = new PlayerDashState();

        playerData = playerDataRaw.Init();
        
        context = new PlayerStateContext
        {
            Controller       = playerController,
            AnimationManager = animationManager,
            LockOnSystem     = lockOnSystem,
            InputManager     = inputManager,
            Rb               = rb,
            CameraTransform  = cameraTransform,
            PlayerTransform  = transform,
            StateMachine     = this,
            PlayerCursor     = playerCursor,
            ShootingSystem   = shootingSystem,
            PlayerData = playerData,
            VfxManager = vfxManager,
            ShootDirection   = transform.forward,
            PlayerHeadTransform = playerHead
        };

        TransitionTo(LocomotionState);
        
        lockOnSystem.InitData(playerData);
        playerController.InitData(playerData);
        shootingSystem.InitData(playerData);

        Mana = mana;
    }

    private void OnEnable()
    {
        inputManager.OnLockToggle += OnLockToggle;
        lockOnSystem.OnUnlock += HandleUnlock;
        
        inputManager.OnJumpPressed += TryJump;
        inputManager.OnAttackMelee += TryAttack;

        playerController.OnAttackMelee += animationManager.AttackMelee;
        playerController.OnJump        += animationManager.Jump;

        shootingSystem.OnChargeTick += uiManager.UpdateEnergyBar;
        
        inputManager.OnDash += TryDash;

        inputManager.OnJumpReleased += TryJumpReleased;
    }

    private void OnDisable()
    {
        inputManager.OnLockToggle  -= OnLockToggle;
        lockOnSystem.OnUnlock -= HandleUnlock;
        
        inputManager.OnJumpPressed -= TryJump;
        inputManager.OnAttackMelee -= TryAttack;
        
        playerController.OnAttackMelee -= animationManager.AttackMelee;
        playerController.OnJump        -= animationManager.Jump;
        
        shootingSystem.OnChargeTick -= uiManager.UpdateEnergyBar;
        
        inputManager.OnDash -= TryDash;
        
        inputManager.OnJumpReleased -= TryJumpReleased;
    }

    private void Start()
    {
        DebugState();
    }

    private void Update()
    {
        CurrentPlayerState.UpdateState(context); 
        TimerDash();
    }

    private void FixedUpdate()
    {
        CurrentPlayerState.FixedUpdateState(context);
    }

    private void LateUpdate()
    {
        playerCursor.FollowPlayer();

        if (lockOnSystem.IsLocked)
        {
            vfxManager.LinkFollow(playerHead,lockOnSystem.CurrentTarget.GetLockTransform());
        }
        else
        {
            vfxManager.ToggleLinkEffect(false);
        }
    }

    #endregion

    #region State Machine

    public void TransitionTo(PlayerBaseState newState)
    {
        CurrentPlayerState?.ExitState(context);
        CurrentPlayerState = newState;
        CurrentPlayerState.EnterState(context);
        DebugState();
    }

    #endregion

    #region Input Gates

    private void TryJump()
    {
        if (!CurrentPlayerState.CanJump) return;
        
        playerController.TryJump();
        TransitionTo(AirState);

    }
    
    private void TryJumpReleased()
    {
        if (CurrentPlayerState is PlayerAirState)
            context.JumpReleased = true;
    }

    private void TryAttack()
    {
        if (!CurrentPlayerState.CanAttack) return;
        
        TransitionTo(MeleeAttackState);
    }


    private void TryDash()
    {
        if (!CurrentPlayerState.CanDash) return;
        if (dashCooldownTimer > 0f) return;
        if (context.HasDash) return;
        
        dashCooldownTimer = dashCooldown;
        TransitionTo(DashState);
    }

    private void TimerDash()
    {
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;
    }

    #endregion

    #region Debugging

    private void DebugState()
    {
        stateText.text = $"State: {CurrentPlayerState.Name}";
    }

    #endregion

    private void OnLockToggle()
    {
        lockOnSystem.ToggleLock();

        if (lockOnSystem.IsLocked)
            vfxManager.ToggleLinkEffect(true, playerHead, lockOnSystem.CurrentTarget.GetLockTransform());
        else
            vfxManager.ToggleLinkEffect(false);
    }

    private void HandleUnlock()
    {
        vfxManager.ToggleLinkEffect(false);
    }
}