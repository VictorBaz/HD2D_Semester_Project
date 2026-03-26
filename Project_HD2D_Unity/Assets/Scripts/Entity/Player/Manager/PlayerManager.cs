using Interface;
using Manager;
using Player.State;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IDamageable
{
    #region Variables

    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerAnimationManager animationManager;
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
    public PlayerAttackState AttackState { get; private set; }
    public PlayerLandingState LandingState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerCarryState CarryState { get; private set; }
    public PlayerHitState HitState { get; private set; }
    public PlayerParryState ParryState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerFallState FallState { get; private set; }

    public PlayerStateContext Context { get; private set; }
    private PlayerDataInstance playerData;

    private float dashCooldownTimer = 0f;
    private float jumpCooldownTimer = 0f;

    public Vector3 TargetDirection { get; private set; } = Vector3.zero;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        LocomotionState = new PlayerLocomotionState();
        AttackState = new PlayerAttackState();
        LandingState = new PlayerLandingState();
        DashState = new PlayerDashState();
        CarryState = new PlayerCarryState();
        HitState = new PlayerHitState();
        ParryState = new PlayerParryState();
        JumpState = new PlayerJumpState();
        FallState = new PlayerFallState();

        playerData = playerDataRaw.Init();

        Context = new PlayerStateContext
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
            PlayerHeadTransform = playerHead,
            TargetDirection = this.TargetDirection,
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
        playerController.OnJump     += animationManager.TriggerJump;

        inputManager.OnAttackMelee += TryAttack;

        inputManager.OnDash += TryDash;

        inputManager.OnEnergyGive += TryGiveEnergy;
        inputManager.OnEnergyTake += TryTakeEnergy;

        inputManager.OnCarry += TryCarry;
        EventManager.OnEject += HandleEject;
        
        inputManager.OnParry += HandleParry;
    }

    private void OnDisable()
    {
        inputManager.OnLockToggle  -= OnLockToggle;
        inputManager.OnLockRelease -= OnLockRelease;

        inputManager.OnJumpPressed  -= TryJump;
        inputManager.OnJumpReleased -= TryJumpReleased;
        playerController.OnJump     -= animationManager.TriggerJump;

        inputManager.OnAttackMelee -= TryAttack;

        inputManager.OnDash -= TryDash;

        inputManager.OnEnergyGive -= TryGiveEnergy;
        inputManager.OnEnergyTake -= TryTakeEnergy;
        
        inputManager.OnCarry -= TryCarry;
        EventManager.OnEject -= HandleEject;
        
        inputManager.OnParry -= HandleParry;
    }

    private void Start()
    {
        DebugState();
    }

    private void Update()
    {
        CurrentPlayerState.UpdateState(Context);
        TickDashTimer();
        TickJumpTimer();
        
        playerController.SetJumping(jumpCooldownTimer > 0);
        
        print(new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude);
    }

    private void FixedUpdate()
    {
        CurrentPlayerState.FixedUpdateState(Context);
    }

    #endregion

    #region State Machine

    public void TransitionTo(PlayerBaseState newState)
    {
        CurrentPlayerState?.ExitState(Context);
        CurrentPlayerState = newState;
        CurrentPlayerState.EnterState(Context);
        DebugState();
    }

    #endregion

    #region Jump

    private void TryJump()
    {
        if (!CurrentPlayerState.CanJump(Context)) return;
        if (jumpCooldownTimer > 0f) return;

        Jump();
    }

    private void Jump()
    {
        jumpCooldownTimer = playerData.JumpCooldown;
        playerController.Jump();
        TransitionTo(JumpState);
    }

    private void TryJumpReleased()
    {
        if (CurrentPlayerState is PlayerInAirBase)
           JumpReleased(); 
    }

    private void JumpReleased()
    {
        Context.JumpReleased = true;
    }
    
    #endregion

    #region Attack

    private void TryAttack()
    {
        if (CurrentPlayerState is PlayerAttackState meleeState)
        {
            meleeState.BufferAttack();
            return;
        }

        if (!CurrentPlayerState.CanAttack) return;
        
        TransitionTo(AttackState);
    }

    #endregion

    #region Link ICarry
    private void TryCarry()
    {
        
        if (Context.CurrentTargetCarry != null)
        {
            Context.CurrentTargetCarry.Eject();
        
            Context.CurrentTargetCarry = null;
        
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

        Context.CurrentTargetCarry = DetectionHelper.GetBestTarget(transform, targets);
        
        if (Context.CurrentTargetCarry != null)
        {
            TransitionTo(CarryState);
        }
    }

    private void HandleEject()
    {
        if (CurrentPlayerState is  PlayerCarryState)
        {
            Context.CurrentTargetCarry = null;    
        }
    }

    

    #endregion

    #region Dash

    private void TryDash()
    {
        if (!CurrentPlayerState.CanDash) return;
        if (dashCooldownTimer > 0f) return;
        if (Context.HasDash) return;

        Dash();
    }

    private void Dash()
    {
        if (CurrentPlayerState is PlayerInAirBase)
        {
            Context.HasDash = true;
        }
        
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

    #region Parry

    private void HandleParry()
    {
        if (lockOnSystem.IsLocked) return;
        if (CurrentPlayerState is PlayerParryState) return; 
        if (!CurrentPlayerState.CanParry) return;
    
        TransitionTo(ParryState);
    }

    #endregion
    
    #region Debug & Gizmos

    private void OnDrawGizmos()
    {
        bool isRuntime = Application.isPlaying && playerData != null;

        Gizmos.color = isRuntime ? (playerController.IsGrounded ? Color.green : Color.red) : Color.yellow;
        
        float height = isRuntime ? playerData.PlayerHeight : playerDataRaw.Movement.PlayerHeight;
        float checkDist = isRuntime ? playerData.GroundCheckDistance : playerDataRaw.Movement.GroundCheckDistance;
        float radius = 0.2f; 

        Vector3 rayStart = transform.position - new Vector3(0, (height / 2) - radius, 0);
        Vector3 rayEnd = rayStart + (Vector3.down * checkDist);

        Gizmos.DrawWireSphere(rayStart, radius);
        Gizmos.DrawLine(rayStart, rayEnd);
        Gizmos.DrawWireSphere(rayEnd, radius);

        Gizmos.color = Color.blue;
        float carryRange = isRuntime ? playerData.CarryRange : playerDataRaw.CarryData.CarryRange;
        float carryAngle = isRuntime ? playerData.CarryAngle : playerDataRaw.CarryData.CarryAngle;

        DrawWireArc(transform.position, transform.forward, carryAngle, carryRange);
    }

    private void DrawWireArc(Vector3 center, Vector3 forward, float angle, float radius)
    {
        Vector3 leftRayRotation = Quaternion.AngleAxis(-angle, Vector3.up) * forward;
        Vector3 rightRayRotation = Quaternion.AngleAxis(angle, Vector3.up) * forward;

        Gizmos.DrawLine(center, center + leftRayRotation * radius);
        Gizmos.DrawLine(center, center + rightRayRotation * radius);
        
        int segments = 10;
        Vector3 previousPoint = center + leftRayRotation * radius;
        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = Mathf.Lerp(-angle, angle, (float)i / segments);
            Vector3 nextPoint = center + (Quaternion.AngleAxis(currentAngle, Vector3.up) * forward) * radius;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }

#endregion

    public void TakeDamage(int value, Vector3 hitDirection)
    {
        if (CurrentPlayerState.CanTakeDamage)
        {
            Context.HitDirection = hitDirection;
            TransitionTo(HitState);
        }
    }

    public Transform GetTransform() => transform;
}