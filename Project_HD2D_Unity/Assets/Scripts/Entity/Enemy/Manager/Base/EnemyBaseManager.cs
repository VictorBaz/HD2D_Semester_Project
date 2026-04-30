using System;
using System.Collections;
using Interface;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class EnemyBaseManager : MonoBehaviour, IDamageableEnemy, ICarryable
{
    #region State Properties
    public EnemyPatrolState    PatrolState    { get; protected set; }
    public EnemyChaseState     ChaseState     { get; protected set; }
    public EnemySearchState    SearchState    { get; protected set; }
    public EnemyGoToSpawnState GoToSpawnState { get; protected set; }
    public EnemyKoState        KoState        { get; protected set; }
    public EnemyHitState       HitState       { get; protected set; }
    public EnemyDropState      DropState      { get; protected set; }
    public EnemyFriendlyState  FriendlyState  { get; protected set; }
    public EnemyExposedState   ExposedState   { get; protected set; }
    public EnemyStaticState    StaticState    { get; protected set; }

    public EnemyBaseState CurrentState      { get; private set; }
    public EnemyBaseState PreviousBaseState { get; private set; }
    public EnemyBaseState AttackState       { get; protected set; }
    #endregion

    #region Serialized Fields
    [Header("Core Components")]
    [SerializeField] protected Rigidbody             rb;
    [SerializeField] protected NavMeshAgent          agent;
    [SerializeField] protected CapsuleCollider              mainCollider;
    [SerializeField] protected EnemyAnimationManager enemyAnimationManager;
    [SerializeField] protected LayerMask enemyLayerMask;
    [SerializeField] protected LayerMask groundLayerMask;
    [SerializeField] protected Transform carryTransform;
    [SerializeField] protected VfxManagerEnemy VfxManager;
    [SerializeField] protected SkinnedMeshRenderer mainRenderer;

    [Header("Triggers")]
    [SerializeField] protected Trigger viewRangeTrigger;
    [SerializeField] protected Trigger attackRangeTrigger;

    [Header("Data & UI")]
    [SerializeField] protected EnemyData enemyData;
    [SerializeField] public    Slider    KoSlider;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints; 
    #endregion

    protected EnemyContext context;
    protected bool         isCarried;
    
    private   bool         isInitialized;
    
    private Vector3 originalPosition;

    private bool isInRecover;
    private Coroutine recoverCoroutine;
    
    public event Action OnTakeDamage;
    
    #region Unity Lifecycle

    protected virtual void Awake()
    {
        InitContext();
        InitializeCommonStates();
        isInitialized = true;

        originalPosition = transform.position;
    }

    protected virtual void Start()
    {
        InitializeState();
        SubscribeEvents();
        
        if (KoSlider != null)
        {
            KoSlider.maxValue = context.Data.MaxKo;
            KoSlider.value    = context.Data.CurrentKo;
        }
        
        agent.speed            = context.Data.PatrolSpeed;
        agent.stoppingDistance = context.Data.StoppingDistance;
        
        ChangeState(PatrolState);
    }

    protected virtual void OnEnable()
    {
        if (!isInitialized) return;
        ChangeState(PatrolState);

        OnTakeDamage += HandleDamageUI;
    }

    protected virtual void Update()
    {
        CurrentState?.UpdateState(context);
    }

    private void OnDestroy() => UnsubscribeEvents();

    #endregion

    #region Initialization

    protected virtual void InitContext()
    {
        context = new EnemyContext
        {
            Manager        = this,
            Agent          = agent,
            Rb             = rb,
            Movement       = GetComponent<EnemyMovement>(),
            AnimManager    = enemyAnimationManager,
            SpawnPosition  = transform.position,
            LastKnownPosition = transform.position,
            LayerMaskEnemy = gameObject.layer,
            GroundLayerMask = groundLayerMask,
            Data           = enemyData.Init(),
            VfxManager = VfxManager,
            CapsuleCollider = mainCollider,
            MainRenderer = mainRenderer,
            PropBlock = new MaterialPropertyBlock(),
        };
    }

    protected virtual void InitializeCommonStates()
    {
        PatrolState    = new EnemyPatrolState();
        ChaseState     = new EnemyChaseState();
        SearchState    = new EnemySearchState();
        GoToSpawnState = new EnemyGoToSpawnState();
        KoState        = new EnemyKoState();
        HitState       = new EnemyHitState();
        DropState      = new EnemyDropState();
        FriendlyState  = new EnemyFriendlyState();
        ExposedState   = new EnemyExposedState();
        StaticState    = new EnemyStaticState();
    }

    protected abstract void InitializeState();

    #endregion

    #region State Machine

    public void ChangeState(EnemyBaseState newState)
    {
        if (newState == null || newState == CurrentState) return;

        CurrentState?.ExitState(context);
        
        PreviousBaseState = CurrentState;
        
        CurrentState      = newState;
        
        CurrentState.EnterState(context);
    }

    #endregion

    #region Trigger Events

    private void SubscribeEvents()
    {
        if (viewRangeTrigger != null)
        {
            viewRangeTrigger.EnteredTrigger += OnViewRangeEnter;
            viewRangeTrigger.ExitedTrigger  += OnViewRangeExit;
        }
        if (attackRangeTrigger != null)
        {
            attackRangeTrigger.EnteredTrigger += OnAttackRangeEnter;
            attackRangeTrigger.ExitedTrigger  += OnAttackRangeExit;
        }
    }

    private void UnsubscribeEvents()
    {
        if (viewRangeTrigger != null)
        {
            viewRangeTrigger.EnteredTrigger -= OnViewRangeEnter;
            viewRangeTrigger.ExitedTrigger  -= OnViewRangeExit;
        }
        if (attackRangeTrigger != null)
        {
            attackRangeTrigger.EnteredTrigger -= OnAttackRangeEnter;
            attackRangeTrigger.ExitedTrigger  -= OnAttackRangeExit;
        }
    }

    protected virtual void OnViewRangeEnter(Collider other)
    {
        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;
        context.Target              = other.gameObject;
        context.IsPlayerInViewRange = true;
    }

    protected virtual void OnViewRangeExit(Collider other)
    {
        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;
        context.LastKnownPosition   = other.transform.position;
        context.Target              = null;
        context.IsPlayerInViewRange = false;
    }

    protected virtual void OnAttackRangeEnter(Collider other)
    {
        if (other.CompareTag(GameConstants.PLAYER_TAG))
            context.IsPlayerInAttackRange = true;
    }

    protected virtual void OnAttackRangeExit(Collider other)
    {
        if (other.CompareTag(GameConstants.PLAYER_TAG))
            context.IsPlayerInAttackRange = false;
    }

    #endregion

    #region Detection

    public bool CanSeePlayer()
    {
        if (context.Target == null) return false;

        Vector3 eyePos = transform.position;
        Vector3 toTarget = (context.Target.transform.position - eyePos);
        float   dist     = toTarget.magnitude;

        bool hit = Physics.Raycast(
            eyePos, toTarget.normalized, out RaycastHit hitInfo,
            dist, ~context.LayerMaskEnemy , QueryTriggerInteraction.Ignore);

        if (hit && hitInfo.transform.CompareTag(GameConstants.PLAYER_TAG))
        {
            context.LastKnownPosition = context.Target.transform.position;
            Debug.DrawLine(eyePos, hitInfo.point, Color.green);
            return true;
        }

        Debug.DrawLine(eyePos, eyePos + toTarget.normalized * dist, Color.red);
        return false;
    }

    #endregion

    #region IDamageable

    public virtual void TakeDamage(int damage, Vector3 hitDirection, int attackType)
    {
        if (isInRecover || (CurrentState != null && !CurrentState.CanTakeDamage)) 
            return;

        context.HitDirection = hitDirection;
        context.Data.CurrentKo += damage;
    
        OnTakeDamage?.Invoke();

        bool isHeavyAttack = (attackType == 2); 

        if (isHeavyAttack || context.Data.IsKoFull())
        {
            ChangeState(HitState);
            return;
        }

        VfxManager.PlayHitVfx();
    }

    public void TakeDamage(int value, Vector3 hitDirection)
    {
        context.HitDirection = hitDirection;
        context.Data.CurrentKo += value;
        OnTakeDamage?.Invoke();
        ChangeState(HitState);
    }

    public Transform GetTransform()          => transform;
    public bool      IsInParryWindow()        => CurrentState != null && CurrentState.CanBeParry;
    public bool      IsInParryWindowPerfect() => false;

    

    #endregion

    #region Parry

    public virtual void HandleParry()
    {
        ChangeState(ExposedState);
    }

    public virtual void HandlePerfectParry()
    {
        ChangeState(ExposedState);
    }

    #endregion

    #region ICarryable

    public bool IsCarryable() => CurrentState == KoState;

    public void Carry(Transform anchor)
    {
        agent.enabled  = false;
        rb.isKinematic = true;
        rb.useGravity  = false;
        mainCollider.enabled = false;

        transform.SetParent(anchor);

        if (carryTransform != null)
        {
            Vector3 offset = transform.position - carryTransform.position;
            transform.localPosition = offset;
        }
        else
        {
            transform.localPosition = Vector3.zero;
        }

        transform.localRotation = Quaternion.identity;
        isCarried = true;
        
        context.AnimManager.SetCarry(true);
        enemyAnimationManager.ToggleRepulsiveCollider(false);
    }

    public void Eject(bool isEscaping = false)
    {
        context.AnimManager.SetCarry(false);
        
        transform.SetParent(null, true);
        mainCollider.enabled = true;

        ApplyMovementMode(true);
        rb.AddForce((transform.forward + Vector3.up) * 5f, ForceMode.Impulse);

        isCarried = false;
        ChangeState(DropState);
        enemyAnimationManager.ToggleRepulsiveCollider(true);
    }

    public bool IsCarry() => isCarried;

    #endregion

    #region Movement Mode

    
    //TODO CHANGE FOR OFFSET BECAUSE STRANGE CLIPPING
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
            float heightOffset = agent.height / 2f; 
            
            Vector3 targetPosition = hit.position + new Vector3(0, heightOffset, 0);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                //transform.position = targetPosition;
                agent.enabled = true;
            }
            else
            {
                agent.enabled = true;
                //agent.Warp(targetPosition);
            }
        }
    }

    public bool IsGrounded(float detectionDistance = 0.1f, float navMeshMargin = 0.1f)
    {
        Vector3 rayOrigin = transform.position;
        float totalDist = detectionDistance;
        
        return Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, totalDist, ~context.LayerMaskEnemy) &&
               NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, navMeshMargin, NavMesh.AllAreas);
    }
    
    public bool IsGroundedDebug(float detectionDistance = 0.1f, float navMeshMargin = 0.1f)
    {
        Vector3 rayOrigin = transform.position;
        float totalDist = detectionDistance;

        
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, totalDist, ~enemyLayerMask))
        {
            return NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, navMeshMargin, NavMesh.AllAreas);
        }
        return false;
    }
    
    private void OnDrawGizmos()
    {
        float testDist = enemyData.Attack.GroundDetectionDistance;
        float testMargin = enemyData.Attack.NavMeshSampleMargin;
    
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.down * (testDist);

        Gizmos.color = IsGroundedDebug(testDist,testMargin) ? Color.green : Color.red;
        Gizmos.DrawLine(start, end);
    
        Gizmos.DrawWireSphere(transform.position, 0.05f);

        Gizmos.color = new Color(1, 1, 0, 0.2f);
        Gizmos.DrawWireSphere(end, testMargin);
    }
    
    #endregion
 
    #region UI need to SRP

    private void HandleDamageUI()
    {
        this.UpdateSlider(KoSlider,context.Data.CurrentKo,0.5f);
    }

    #endregion
    
    public void ResetEnemy()
    {
        transform.position = originalPosition;
        VfxManager.StopKoVfx();
        context.Data.ResetKo();
        HandleDamageUI();
        ChangeState(PatrolState);
    }

    public void RecoverPhase()
    {
        if(recoverCoroutine != null) StopCoroutine(recoverCoroutine);
        recoverCoroutine = StartCoroutine(RecoverPhaseIe());
    }
    
    private IEnumerator RecoverPhaseIe()
    {
        isInRecover = true;
        yield return new WaitForSeconds(2);
        isInRecover = false;
    }
}