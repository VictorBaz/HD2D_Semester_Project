using Interface;
using Manager;
using Player.State;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private AnimationManager animationManager;
    [SerializeField] private LockOnSystem lockOnSystem;
    [SerializeField] private UiManager uiManager;
    [SerializeField] private VfxManager vfxManager;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform playerHead;
    
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private Rigidbody rb;
    
    [SerializeField] private PlayerData playerDataRaw;

    public PlayerBaseState CurrentPlayerState { get; private set; }
    public PlayerLocomotionState LocomotionState { get; private set; }
    public PlayerAirState AirState { get; private set; }
    public PlayerAttackMeleeState MeleeAttackState { get; private set; }
    public PlayerLandingState LandingState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerCarryState CarryState { get; private set; }

    private PlayerStateContext context;
    private PlayerDataInstance playerData;

    private float dashCooldownTimer = 0f;
    private float jumpCooldownTimer = 0f;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        LocomotionState = new PlayerLocomotionState();
        AirState = new PlayerAirState();
        MeleeAttackState = new PlayerAttackMeleeState();
        LandingState = new PlayerLandingState();
        DashState = new PlayerDashState();
        CarryState = new PlayerCarryState();

        playerData = playerDataRaw.Init();

        context = new PlayerStateContext
        {
            Controller = playerController,
            AnimationManager = animationManager,
            LockOnSystem = lockOnSystem,
            InputManager = inputManager,
            Rb = rb,
            CameraTransform = cameraTransform,
            PlayerTransform = transform,
            StateMachine = this,
            PlayerData = playerData,
            VfxManager = vfxManager,
            ShootDirection = transform.forward,
            PlayerHeadTransform = playerHead
        };

        TransitionTo(LocomotionState);

        lockOnSystem.InitData(playerData);
        playerController.InitData(playerData);
        
        uiManager.UpdateEnergyTxt(playerData.Energy);
    }

    private void OnEnable()
    {
        inputManager.OnLockToggle  += OnLockToggle;
        inputManager.OnLockRelease += OnLockRelease;

        inputManager.OnJumpPressed  += TryJump;
        inputManager.OnJumpReleased += TryJumpReleased;
        playerController.OnJump     += animationManager.Jump;

        inputManager.OnAttackMelee += TryAttack;

        inputManager.OnDash += TryDash;

        inputManager.OnEnergyGive += TryGiveEnergy;
        inputManager.OnEnergyTake += TryTakeEnergy;

        inputManager.OnCarry += TryCarry;
    }

    private void OnDisable()
    {
        inputManager.OnLockToggle  -= OnLockToggle;
        inputManager.OnLockRelease -= OnLockRelease;

        inputManager.OnJumpPressed  -= TryJump;
        inputManager.OnJumpReleased -= TryJumpReleased;
        playerController.OnJump     -= animationManager.Jump;

        inputManager.OnAttackMelee -= TryAttack;

        inputManager.OnDash -= TryDash;

        inputManager.OnEnergyGive -= TryGiveEnergy;
        inputManager.OnEnergyTake -= TryTakeEnergy;
        
        inputManager.OnCarry += TryCarry;
    }

    private void Start()
    {
        DebugState();
    }

    private void Update()
    {
        CurrentPlayerState.UpdateState(context);
        TickDashTimer();
        TickJumpTimer();
    }

    private void FixedUpdate()
    {
        CurrentPlayerState.FixedUpdateState(context);
    }

    /*private void LateUpdate()
    {
        if (lockOnSystem.IsLocked)
            vfxManager.LinkFollow(playerHead, lockOnSystem.CurrentTarget.GetLockTransform());
        else
            vfxManager.ToggleLinkEffect(false);
    }*/

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

    #region Jump

    private void TryJump()
    {
        if (!CurrentPlayerState.CanJump(context)) return;
        if (jumpCooldownTimer > 0f) return;

        Jump();
    }

    private void Jump()
    {
        jumpCooldownTimer = playerData.JumpCooldown;
        playerController.Jump();
        TransitionTo(AirState);
    }

    private void TryJumpReleased()
    {
        if (CurrentPlayerState is PlayerAirState)
           JumpReleased(); 
    }

    private void JumpReleased()
    {
        context.JumpReleased = true;
    }
    
    #endregion

    #region Attack

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

    #endregion

    private void TryCarry()
    {
        
        if (context.CurrentTargetCarry != null)
        {
            TransitionTo(LocomotionState);
            return;
        }

        if (!CurrentPlayerState.CanCarry) return;
        
        var targets = DetectionHelper.FindVisibleTargets<ICarryable>(
            transform, 
            playerData.CarryRange, 
            playerData.CarryAngle, 
            playerData.CarryLayer
        );

        targets.RemoveAll(t => !t.IsCarryable());

        context.CurrentTargetCarry = DetectionHelper.GetBestTarget(transform, targets);
        
        if (context.CurrentTargetCarry != null)
        {
            TransitionTo(CarryState);
        }
    }

    #region Dash

    private void TryDash()
    {
        if (!CurrentPlayerState.CanDash) return;
        if (dashCooldownTimer > 0f) return;
        if (context.HasDash) return;

        Dash();
    }

    private void Dash()
    {
        dashCooldownTimer = playerData.DashCooldown;
        TransitionTo(DashState);
    }

    #endregion

    #region Energy

    private void TryGiveEnergy()
    {
        if (!CanInteractWithTarget(out IEnergyLockable target)) return;
        if (playerData.IsEnergyEmpty()) return;
        if (target.IsAtMaximumEnergy()) return;

        HandleEnergy(target, false);
    }

    private void TryTakeEnergy()
    {
        if (!CanInteractWithTarget(out IEnergyLockable target)) return;
        if (!target.IsContainingEnergy()) return;

        HandleEnergy(target, true);
    }

    private bool CanInteractWithTarget(out IEnergyLockable target)
    {
        target = null;
        
        if (!lockOnSystem.IsLocked) return false;
        if (lockOnSystem.CurrentTarget is not IEnergyLockable energyTarget) return false;
        
        target = energyTarget;
        
        return true;
    }

    private void HandleEnergy(IEnergyLockable target, bool takingEnergy)
    {
        if (takingEnergy)
        {
            target.RemoveEnergy(); 
            playerData.AddEnergy();
        }
        else
        {
            target.AddEnergy();    
            playerData.RemoveEnergy();
        }

        uiManager.UpdateEnergyTxt(playerData.Energy);
    }

    #endregion

    #region Timers

    private void TickDashTimer()
    {
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;
    }

    private void TickJumpTimer()
    {
        if (jumpCooldownTimer > 0f)
            jumpCooldownTimer -= Time.deltaTime;
    }

    #endregion

    #region Lock On

    private void OnLockToggle()
    {
        lockOnSystem.TryLock();

        /*if (lockOnSystem.IsLocked)
            vfxManager.ToggleLinkEffect(true, playerHead, lockOnSystem.CurrentTarget.GetLockTransform());
        else
            vfxManager.ToggleLinkEffect(false);*/
    }

    private void OnLockRelease()
    {
        lockOnSystem.Unlock();
        /*vfxManager.ToggleLinkEffect(false);*/
    }

    #endregion

    #region Debugging

    private void DebugState()
    {
        stateText.text = $"State: {CurrentPlayerState.Name}";
    }

    #endregion
}