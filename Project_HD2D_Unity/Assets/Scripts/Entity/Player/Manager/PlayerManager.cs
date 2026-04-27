using Interface;
using Player.State;
using UnityEngine;

[RequireComponent(typeof(PlayerActionHandler))]
public class PlayerManager : MonoBehaviour, IDamageable, IDataPersistence
{
    #region Variables

    [SerializeField] private PlayerController  playerController;
    [SerializeField] private PlayerAnimationManager animationManager;
    [SerializeField] private InputManager      inputManager;
    [SerializeField] private LockOnSystem      lockOnSystem;
    [SerializeField] private VfxManagerPlayer  vfxManagerPlayer;
    [SerializeField] private Transform         cameraTransform;
    [SerializeField] private Transform         playerHead;
    [SerializeField] private Rigidbody         rb;
    [SerializeField] private CapsuleCollider   playerCollider;
    [SerializeField] private PlayerData        playerDataRaw;

    public PlayerBaseState     CurrentPlayerState { get; private set; }
    public PlayerLocomotionState LocomotionState  { get; private set; }
    public PlayerAttackState     AttackState      { get; private set; }
    public PlayerLandingState    LandingState     { get; private set; }
    public PlayerDashState       DashState        { get; private set; }
    public PlayerCarryState      CarryState       { get; private set; }
    public PlayerHitState        HitState         { get; private set; }
    public PlayerParryState      ParryState       { get; private set; }
    public PlayerJumpState       JumpState        { get; private set; }
    public PlayerFallState       FallState        { get; private set; }
    public PlayerBumpState       BumpState        { get; private set; }

    public PlayerStateContext Context    { get; private set; }

    private PlayerDataInstance playerData;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        InitStates();

        playerData = playerDataRaw.Init();

        Context = new PlayerStateContext
        {
            Controller        = playerController,
            AnimationManager  = animationManager,
            LockOnSystem      = lockOnSystem,
            InputManager      = inputManager,
            Rb                = rb,
            CameraTransform   = cameraTransform,
            PlayerTransform   = transform,
            StateMachine      = this,
            PlayerData        = playerData,
            VfxManagerPlayer  = vfxManagerPlayer,
            ShootDirection    = transform.forward,
            PlayerHeadTransform = playerHead,
            Collider          = playerCollider
        };

        TransitionTo(LocomotionState);

        lockOnSystem.InitData(playerData);
        playerController.InitData(playerData);

        PlayerEvents.OnRequestPlayerTransform = GetTransform;
        PlayerEvents.OnRequestPlayerContext   = GetContext;
    }

    private void Start()
    {
        UiEvents.TriggerEnergySetup(Context.PlayerData.MaxEnergy, Context.PlayerData.Energy);
        UiEvents.TriggerSapChanged(Context.PlayerData.Sap);
    }

    private void Update()
    {
        CurrentPlayerState.UpdateState(Context);
    }

    private void FixedUpdate()
    {
        CurrentPlayerState.FixedUpdateState(Context);
    }

    private void OnDestroy()
    {
        PlayerEvents.OnRequestPlayerTransform  = null;
        PlayerEvents.OnRequestPlayerContext    = null;
        PlayerEvents.OnRequestCurrentLockTarget = null;
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
        CurrentPlayerState?.ExitState(Context);
        CurrentPlayerState = newState;
        CurrentPlayerState.EnterState(Context);
    }

    #endregion

    #region IDamageable

    public void TakeDamage(int value, Vector3 hitDirection)
    {
        if (!CurrentPlayerState.CanTakeDamage)    return;
        if (CurrentPlayerState.IsParryWindowActive) return;

        Context.HitDirection = hitDirection;
        TransitionTo(HitState);
    }

    public Transform GetTransform() => transform;

    public bool IsInParryWindow()
        => CurrentPlayerState is PlayerParryState p && p.IsParryWindowActive;

    public bool IsInParryWindowPerfect()
        => CurrentPlayerState is PlayerParryState p && p.IsPerfectWindowActive;

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
        data.PlayerData = new PlayerSaveData(playerData);
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        bool isRuntime = Application.isPlaying && playerData != null;

        Gizmos.color = isRuntime
            ? (playerController.IsGrounded ? Color.green : Color.red)
            : Color.yellow;

        float height    = isRuntime ? playerData.PlayerHeight          : playerDataRaw.Movement.PlayerHeight;
        float checkDist = isRuntime ? playerData.GroundCheckDistance   : playerDataRaw.Movement.GroundCheckDistance;
        float radius    = 0.2f;

        Vector3 rayStart = transform.position - new Vector3(0, (height / 2) - radius, 0);
        Vector3 rayEnd   = rayStart + Vector3.down * checkDist;

        Gizmos.DrawWireSphere(rayStart, radius);
        Gizmos.DrawLine(rayStart, rayEnd);
        Gizmos.DrawWireSphere(rayEnd, radius);

        Gizmos.color = Color.blue;
        float carryRange = isRuntime ? playerData.CarryRange : playerDataRaw.Abilities.CarryRange;
        float carryAngle = isRuntime ? playerData.CarryAngle : playerDataRaw.Abilities.CarryAngle;

        DrawWireArc(transform.position, transform.forward, carryAngle, carryRange);
    }

    private void DrawWireArc(Vector3 center, Vector3 forward, float angle, float radius)
    {
        Vector3 left  = Quaternion.AngleAxis(-angle, Vector3.up) * forward;
        Vector3 right = Quaternion.AngleAxis( angle, Vector3.up) * forward;

        Gizmos.DrawLine(center, center + left  * radius);
        Gizmos.DrawLine(center, center + right * radius);

        int segments = 10;
        Vector3 prev = center + left * radius;
        for (int i = 1; i <= segments; i++)
        {
            float   a    = Mathf.Lerp(-angle, angle, (float)i / segments);
            Vector3 next = center + (Quaternion.AngleAxis(a, Vector3.up) * forward) * radius;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }

    #endregion

    private PlayerStateContext GetContext() => Context;
}