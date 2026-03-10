using System;
using System.Diagnostics;
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
    [SerializeField] private Transform shootOriginPoint;

    [SerializeField] private PlayerCursor   playerCursor;
    [SerializeField] private Rigidbody      rb;
    [SerializeField] private ShootingSystem shootingSystem;
    [SerializeField] private PlayerData playerDataRaw;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private VfxManager vfxManager;

    public PlayerBaseState CurrentPlayerState { get; private set; }

    private PlayerStateContext context;
    
    private PlayerDataInstance playerData;
    
    //TEMPORARY

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
            VfxManager = vfxManager
        };

        TransitionTo(new PlayerLocomotionState());
        
        lockOnSystem.InitData(playerData);
        playerController.InitData(playerData);
        shootingSystem.InitData(playerData);

        Mana = mana;
    }

    private void OnEnable()
    {
        inputManager.OnLockToggle  += lockOnSystem.ToggleLock;
        inputManager.OnJumpPressed += TryJump;
        inputManager.OnAttackMelee += TryAttack;

        inputManager.OnShootStart += TryStartShoot;
        inputManager.OnShootStop  += TryStopShoot;

        playerController.OnAttackMelee += animationManager.AttackMelee;
        playerController.OnJump        += animationManager.Jump;

        shootingSystem.OnChargeTick += uiManager.UpdateEnergyBar;
        
        inputManager.OnDash += TryDash;
    }

    private void OnDisable()
    {
        inputManager.OnLockToggle  -= lockOnSystem.ToggleLock;
        inputManager.OnJumpPressed -= TryJump;
        inputManager.OnAttackMelee -= TryAttack;

        inputManager.OnShootStart -= TryStartShoot;
        inputManager.OnShootStop  -= TryStopShoot;

        playerController.OnAttackMelee -= animationManager.AttackMelee;
        playerController.OnJump        -= animationManager.Jump;
        
        shootingSystem.OnChargeTick -= uiManager.UpdateEnergyBar;
        
        inputManager.OnDash += TryDash;
    }

    private void Start()
    {
        DebugState();
    }

    private void Update()
    {
        CurrentPlayerState.UpdateState(context); ;
    }

    private void FixedUpdate()
    {
        CurrentPlayerState.FixedUpdateState(context);
    }

    private void LateUpdate()
    {
        playerCursor.FollowPlayer();
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
        TransitionTo(new PlayerAirState());

    }

    private void TryAttack()
    {
        if (!CurrentPlayerState.CanAttack) return;
        
        TransitionTo(new PlayerAttackMeleeState());
    }

    private void TryStartShoot()
    {
        if (CurrentPlayerState.CanShoot && mana > 0)
        {
            shootingSystem.HandleStartTryShoot();
        }
    }

    private void TryStopShoot()
    {
        if (!CurrentPlayerState.CanShoot) return;

        Transform lockTarget = lockOnSystem.IsLocked 
            ? lockOnSystem.CurrentTarget.GetLockTransform() 
            : null;

        shootingSystem.HandleStopTryShoot(lockTarget);
        Mana--;
    }

    private void TryDash()
    {
        if (CurrentPlayerState.CanMove)
        {
            TransitionTo(new PlayerDashState());
        }
    }

    #endregion

    #region Debugging

    private void DebugState()
    {
        stateText.text = $"State: {CurrentPlayerState.Name}";
    }

    #endregion
}