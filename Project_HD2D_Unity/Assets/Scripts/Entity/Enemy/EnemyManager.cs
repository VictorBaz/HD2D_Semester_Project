using System;
using Interface;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour, IDamageable, ICarryable
{
    #region State Properties
    public EnemyPatrolState    PatrolState       { get; private set; }
    public EnemyChaseState     ChaseState        { get; private set; }
    public EnemyAttackState    AttackState       { get; private set; }
    public EnemySearchState    SearchState       { get; private set; }
    public EnemyGoToSpawnState GoToSpawnState    { get; private set; }
    public EnemyKoState        KoState           { get; private set; }
    public EnemyHitState       HitState          { get; private set; }
    public EnemyDropState      DropState         { get; private set; }
    public EnemyFriendlyState  FriendlyState     { get; private set; }
    public EnemyExposedState   ExposedState      { get; private set; }
    
    public EnemyBaseState      PreviousBaseState { get; private set; }
    #endregion

    #region Serialized Fields
    [Header("Core Components")]
    [SerializeField] private Rigidbody             rb;
    [SerializeField] private NavMeshAgent          agent;
    [SerializeField] private Collider              mainCollider;
    [SerializeField] private EnemyAnimationManager enemyAnimationManager;

    [Header("Triggers")]
    [SerializeField] private Trigger viewRangeTrigger;
    [SerializeField] private Trigger attackRangeTrigger;

    [Header("Detection Settings")]
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float     eyeHeightOffset = 1.5f;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;

    [Header("UI & Feedback")]
    public Slider   KoSlider;
    public TMP_Text StateTxt;
    [SerializeField] private Image feedbackRenderer;

    [Header("Data")]
    [SerializeField] private EnemyData enemyDataRaw;
    #endregion

    #region Private Fields
    private EnemyBaseState    currentState;
    private EnemyContext      context;
    private EnemyMovement     movement;
    private EnemyDataInstance data;

    private GameObject target;
    private Vector3    spawnPosition;
    private bool       isCarried;
    private bool       isInitialized;

    private bool isPlayerInAttackRange;
    private bool isPlayerInViewRange;
    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        data          = enemyDataRaw.Init();
        movement      = GetComponent<EnemyMovement>();
        spawnPosition = transform.position;

        InitializeStates();
        SubscribeEvents();
        BuildContext();

        isInitialized = true;
    }

    private void Start()
    {
        if (KoSlider != null)
        {
            KoSlider.maxValue = data.MaxKo;
            KoSlider.value    = data.CurrentKo;
        }

        if (StateTxt != null)
            StateTxt.text = "INIT";

        agent.speed            = data.PatrolSpeed;
        agent.stoppingDistance = data.StoppingDistance;
        agent.acceleration     = 12f;

        ChangeState(PatrolState);
    }

    private void OnEnable()
    {
        if (!isInitialized) return;

        movement?.StopMovement();
        transform.position = spawnPosition;
        ChangeState(PatrolState);
    }

    private void Update()
    {
        context.Target                = target;
        context.IsPlayerInViewRange   = isPlayerInViewRange;
        context.IsPlayerInAttackRange = isPlayerInAttackRange;

        currentState?.UpdateState(context);

        Debug.DrawRay(context.LastKnownPosition, Vector3.up * 2f, Color.yellow);
    }

    private void OnDestroy() => UnsubscribeEvents();

    #endregion

    #region Initialization Helpers

    private void InitializeStates()
    {
        PatrolState    = new EnemyPatrolState();
        ChaseState     = new EnemyChaseState();
        AttackState    = new EnemyAttackState();
        SearchState    = new EnemySearchState();
        GoToSpawnState = new EnemyGoToSpawnState();
        KoState        = new EnemyKoState();
        HitState       = new EnemyHitState();
        DropState      = new EnemyDropState();
        FriendlyState  = new EnemyFriendlyState();
        ExposedState   = new EnemyExposedState();
    }

    private void BuildContext()
    {
        context = new EnemyContext
        {
            Manager           = this,
            Agent             = agent,
            Rb                = rb,
            Movement          = movement,
            SpawnPosition     = spawnPosition,
            LastKnownPosition = spawnPosition,
            Data              = data,
            AnimManager       = enemyAnimationManager,
            LayerMaskEnemy    = gameObject.layer,
        };
    }

    #endregion

    #region State Machine

    public void ChangeState(EnemyBaseState newState)
    {
        currentState?.ExitState(context);
        PreviousBaseState = currentState;
        currentState      = newState;

        if (StateTxt != null)
            StateTxt.text = newState.Name;

        if (feedbackRenderer != null && data != null)
            feedbackRenderer.sprite = data.GetSprite(currentState);

        currentState.EnterState(context);
    }

    #endregion

    #region Detection

    public bool CanSeePlayer()
    {
        if (target == null) return false;

        Vector3 eyePos   = transform.position + Vector3.up * eyeHeightOffset;
        Vector3 toTarget = target.transform.position - eyePos;
        float   dist     = toTarget.magnitude;

        bool hit = Physics.Raycast(
            eyePos, toTarget.normalized, out RaycastHit hitInfo,
            dist, obstacleMask, QueryTriggerInteraction.Ignore);

        if (hit && hitInfo.transform.CompareTag(GameConstants.PLAYER_TAG))
        {
            context.LastKnownPosition = target.transform.position;
            Debug.DrawLine(eyePos, hitInfo.point, Color.green);
            return true;
        }

        Debug.DrawLine(eyePos, eyePos + toTarget.normalized * dist, Color.red);
        return false;
    }

    #endregion

    #region Trigger Events

    private void SubscribeEvents()
    {
        if (viewRangeTrigger != null)
        {
            viewRangeTrigger.EnteredTrigger += OnViewEntered;
            viewRangeTrigger.ExitedTrigger  += OnViewExited;
        }
        if (attackRangeTrigger != null)
        {
            attackRangeTrigger.EnteredTrigger += OnAttackEntered;
            attackRangeTrigger.ExitedTrigger  += OnAttackExited;
        }
    }

    private void UnsubscribeEvents()
    {
        if (viewRangeTrigger != null)
        {
            viewRangeTrigger.EnteredTrigger -= OnViewEntered;
            viewRangeTrigger.ExitedTrigger  -= OnViewExited;
        }
        if (attackRangeTrigger != null)
        {
            attackRangeTrigger.EnteredTrigger -= OnAttackEntered;
            attackRangeTrigger.ExitedTrigger  -= OnAttackExited;
        }
    }

    private void OnViewEntered(Collider other)
    {
        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;
        target              = other.gameObject;
        isPlayerInViewRange = true;
    }

    private void OnViewExited(Collider other)
    {
        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;
        target              = null;
        isPlayerInViewRange = false;
    }

    private void OnAttackEntered(Collider other)
    {
        if (other.CompareTag(GameConstants.PLAYER_TAG))
            isPlayerInAttackRange = true;
    }

    private void OnAttackExited(Collider other)
    {
        if (other.CompareTag(GameConstants.PLAYER_TAG))
            isPlayerInAttackRange = false;
    }

    #endregion

    #region IDamageable

    public void TakeDamage(int amount, Vector3 direction)
    {
        if (!currentState.CanTakeDamage) return;

        data.CurrentKo += amount;

        if (KoSlider != null)
        {
            KoSlider.maxValue = data.MaxKo;
            this.UpdateSlider(KoSlider, data.CurrentKo);
        }

        context.HitDirection = direction;
        ChangeState(HitState);
    }

    public Transform GetTransform()    => transform;
    public bool      IsInParryWindow() => false;

    #endregion

    #region Parry

    public void HandleParry()
    {
        ChangeState(ExposedState);
    }

    #endregion

    #region ICarryable

    public bool IsCarryable() => currentState == KoState;

    public void Carry(Transform anchor)
    {
        agent.enabled  = false;
        rb.isKinematic = true;
        rb.useGravity  = false;

        transform.SetParent(anchor);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        mainCollider.enabled = false;
        isCarried            = true;
    }

    public void Eject(bool isEscaping = false)
    {
        transform.SetParent(null, true);
        mainCollider.enabled = true;

        ApplyMovementMode(usePhysics: true);

        rb.AddForce((transform.forward + Vector3.up) * 5f, ForceMode.Impulse);

        isCarried = false;
        ChangeState(DropState);
    }

    public bool IsCarry() => isCarried;

    #endregion

    #region Movement Mode

    public void ApplyMovementMode(bool usePhysics)
    {
        if (usePhysics)
        {
            agent.enabled  = false;
            rb.isKinematic = false;
            rb.useGravity  = true;
            return;
        }

        if (!rb.isKinematic)
        {
            rb.linearVelocity  = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        rb.isKinematic = true;
        rb.useGravity  = false;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            agent.enabled = true;
            agent.Warp(hit.position);
        }
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 start = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawLine(start, start + Vector3.down * 1.2f);
    }

    #endregion
}