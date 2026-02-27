using Player.State;
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

    public PlayerBaseState CurrentPlayerState { get; private set; }

    private PlayerStateContext context;
    
    [SerializeField] private PlayerData playerDataRaw;
    private PlayerDataInstance playerData;

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
            PlayerData = playerData
        };

        TransitionTo(new PlayerLocomotionState());
        
        lockOnSystem.InitData(playerData);
        playerController.InitData(playerData);
        shootingSystem.InitData(playerData);
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
    }

    private void Update()
    {
        CurrentPlayerState.UpdateState(context);
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
        if (CurrentPlayerState.CanShoot)
        {
            shootingSystem.HandleStartTryShoot();
        }
    }

    private void TryStopShoot()
    {
        if (CurrentPlayerState.CanShoot)
        {
            shootingSystem.HandleStopTryShoot();
        }
    }

    #endregion
}