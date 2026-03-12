using System;
using System.Diagnostics;
using System.Timers;
using Manager;
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
    [SerializeField] private Rigidbody      rb;
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

    private float dashCooldownTimer = 0f;
    private float jumpCooldownTimer = 0f;
    
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
            PlayerData = playerData,
            VfxManager = vfxManager,
            ShootDirection   = transform.forward,
            PlayerHeadTransform = playerHead
        };

        TransitionTo(LocomotionState);
        
        lockOnSystem.InitData(playerData);
        playerController.InitData(playerData);

        Mana = mana;
    }

    private void OnEnable()
    {
        inputManager.OnLockToggle += OnLockToggle;
        inputManager.OnLockToggle  += OnLockToggle;
        inputManager.OnLockRelease += OnLockRelease;
        
        inputManager.OnJumpPressed += TryJump;
        inputManager.OnJumpReleased += TryJumpReleased;
        playerController.OnJump += animationManager.Jump;
        
        inputManager.OnAttackMelee += TryAttack;
        playerController.OnAttackMelee += animationManager.AttackMelee;
        
        inputManager.OnDash += TryDash;

        inputManager.OnEnergyGive += TryGiveEnergy;
        inputManager.OnEnergyTake += TryTakeEnergy;
    }

    private void OnDisable()
    {
        inputManager.OnLockToggle  -= OnLockToggle;
        inputManager.OnLockToggle  -= OnLockToggle;
        inputManager.OnLockRelease -= OnLockRelease;
        
        inputManager.OnJumpPressed -= TryJump;
        inputManager.OnJumpReleased -= TryJumpReleased;
        playerController.OnJump -= animationManager.Jump;
        
        inputManager.OnAttackMelee -= TryAttack;
        playerController.OnAttackMelee -= animationManager.AttackMelee;
        
        inputManager.OnDash -= TryDash;
        
        inputManager.OnEnergyGive -= TryGiveEnergy;
        inputManager.OnEnergyTake -= TryTakeEnergy;
    }

    private void Start()
    {
        DebugState();
    }

    private void Update()
    {
        CurrentPlayerState.UpdateState(context); 
        TimerDash();
        TimerJump();
    }

    private void FixedUpdate()
    {
        CurrentPlayerState.FixedUpdateState(context);
    }

    private void LateUpdate()
    {
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

    private void TryTakeEnergy()
    {
        if (!lockOnSystem.IsLocked) return;

        lockOnSystem.CurrentTarget.AddEnergy();
    }

    private void TryGiveEnergy()
    {
        if (!lockOnSystem.IsLocked) return;

        lockOnSystem.CurrentTarget.RemoveEnergy();
    }
    

    private void TryJump()
    {
        if (!CurrentPlayerState.CanJump || jumpCooldownTimer > 0f || lockOnSystem.IsLocked) return;

        jumpCooldownTimer = playerData.JumpCooldown;
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
        if (CurrentPlayerState is PlayerAttackMeleeState meleeState)
        {
            meleeState.BufferAttack();
            return;
        }

        if (!CurrentPlayerState.CanAttack) return;
        TransitionTo(MeleeAttackState);
    }


    private void TryDash()
    {
        if (!CurrentPlayerState.CanDash) return;
        if (dashCooldownTimer > 0f) return;
        if (context.HasDash) return;
        
        dashCooldownTimer = playerData.DashCooldown;
        TransitionTo(DashState);
    }

    #endregion
    
    #region Timer
    
    private void TimerDash()
    {
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;
    }
    
    private void TimerJump()
    {
        if (jumpCooldownTimer > 0f)
            jumpCooldownTimer -= Time.deltaTime;
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
        lockOnSystem.TryLock();

        if (lockOnSystem.IsLocked)
            vfxManager.ToggleLinkEffect(true, playerHead, lockOnSystem.CurrentTarget.GetLockTransform());
        else
            vfxManager.ToggleLinkEffect(false);
    }

    private void OnLockRelease()
    {
        lockOnSystem.Unlock();
        vfxManager.ToggleLinkEffect(false);
    }
}