using System.Collections;
using Interface;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBehavior : MonoBehaviour, IDamageableEnemy, ICarryable
{
    #region Variables
    //public AiStatic StaticState { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }
    public EnemySearchState SearchState { get; private set; }
    public EnemyGoToSpawnState GoToSpawnState { get; private set; }
    public EnemyKoState KoState { get; private set; }
    public EnemyHitState EnemyHit { get; private set; }
    public EnemyDropState DropState { get; private set; }
    public EnemyFriendlyState FriendlyState { get; private set; }
    public EnemyExposedState ExposedState { get; private set; }

    [Header("Core Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Collider mainCollider;
    [SerializeField] private EnemyAnimationManager enemyAnimationManager;

    [Header("Triggers")]
    [SerializeField] private Trigger viewRangeTrigger;
    [SerializeField] private Trigger attackRangeTrigger;

    [Header("Detection Settings")]
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float eyeHeightOffset = 1.5f;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;

    [Header("UI & Feedback")]
    public Slider KoSlider;
    public TMP_Text StateTxt;
    [SerializeField] private Image feedbackRenderer;

    private EnemyBaseState _currentBaseState;
    private EnemyContext context;
    private bool isCarry = false;

    [SerializeField] private EnemyData enemyDataRaw;
    private EnemyDataInstance data;

    public EnemyBaseState PreviousBaseState {get; private set;}
    private EnemyMovement movement;
    private GameObject target;
    private Vector3 spawnPosition;
    private Vector3 lastKnownPosition;
    private bool isPlayerInAttackRange;
    private bool isPlayerInViewRange;
    public bool isFriendly { get; private set; } = false;
    #endregion

    #region Unity Lifecycle
    void Awake()
    {
        data = enemyDataRaw.Init();
        
        PatrolState = new EnemyPatrolState();
        ChaseState = new EnemyChaseState();
        AttackState = new EnemyAttackState();
        SearchState = new EnemySearchState();
        GoToSpawnState = new EnemyGoToSpawnState();
        KoState = new EnemyKoState();
        EnemyHit = new EnemyHitState();
        DropState = new EnemyDropState();
        FriendlyState = new EnemyFriendlyState();
        ExposedState = new EnemyExposedState();

        movement = GetComponent<EnemyMovement>();
        spawnPosition = transform.position;
        lastKnownPosition = spawnPosition;
        
        SubscribeEvents();
        
        context = new EnemyContext
        {
            Behavior = this,
            Agent = agent,
            Rb = rb,
            Movement = movement,
            SpawnPosition = spawnPosition,
            LastKnownPosition = lastKnownPosition,
            Data = data,
            AnimManager = enemyAnimationManager,
            LayerMaskEnemy = this.gameObject.layer,
            
        };
    }

    void Start()
    {
        if (KoSlider != null) KoSlider.maxValue = data.MaxKo;
        if (StateTxt != null) StateTxt.text = "INIT";

        KoSlider.value = data.CurrentKo;
        
        agent.speed = data.PatrolSpeed;
        agent.stoppingDistance = data.StoppingDistance;
        agent.acceleration = 12f;
        
        ChangeState(PatrolState);
    }

    void OnEnable()
    {
        if (movement != null) movement.StopMovement();
        transform.position = spawnPosition;
        ChangeState(PatrolState);
    }

    void Update() 
    {
        context.Target = target;
        context.IsPlayerInViewRange = isPlayerInViewRange;
        context.IsPlayerInAttackRange = isPlayerInAttackRange;
        context.LastKnownPosition = lastKnownPosition;
        
        _currentBaseState?.UpdateState(context);
        
       
        
        Debug.DrawRay(lastKnownPosition, Vector3.up * 2f, Color.yellow);
    }

    void OnDestroy() => UnsubscribeEvents();
    #endregion

    #region State Logic
    public void ChangeState(EnemyBaseState newBaseState)
    {
        _currentBaseState?.ExitState(context);
        PreviousBaseState = _currentBaseState;
        
        _currentBaseState = newBaseState;
        if (StateTxt != null) StateTxt.text = newBaseState.Name;
        
        if (feedbackRenderer != null && data != null)
        {
            feedbackRenderer.sprite = data.GetSprite(_currentBaseState);
        }
        
        _currentBaseState.EnterState(context);
    }
    #endregion

    #region Detection Logic
    public bool CanSeePlayer()
    {
        if (target == null) return false;

        Vector3 eyePos = transform.position + Vector3.up * eyeHeightOffset;
        Vector3 dir = (target.transform.position - eyePos).normalized;
        float dist = Vector3.Distance(eyePos, target.transform.position);

        if (Physics.Raycast(eyePos, dir, out RaycastHit hit, dist, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.CompareTag("Player")) 
            {
                lastKnownPosition = target.transform.position;
                Debug.DrawLine(eyePos, hit.point, Color.green);
                return true;
            }
        }
        
        Debug.DrawLine(eyePos, eyePos + dir * dist, Color.red);
        return false;
    }
    #endregion

    #region Event Management
    private void SubscribeEvents()
    {
        if (viewRangeTrigger != null)
        {
            viewRangeTrigger.EnteredTrigger += OnViewEntered;
            viewRangeTrigger.ExitedTrigger += OnViewExited;
        }
        if (attackRangeTrigger != null)
        {
            attackRangeTrigger.EnteredTrigger += OnAttackEntered;
            attackRangeTrigger.ExitedTrigger += OnAttackExited;
        }
    }

    private void UnsubscribeEvents()
    {
        if (viewRangeTrigger != null)
        {
            viewRangeTrigger.EnteredTrigger -= OnViewEntered;
            viewRangeTrigger.ExitedTrigger -= OnViewExited;
        }
        if (attackRangeTrigger != null)
        {
            attackRangeTrigger.EnteredTrigger -= OnAttackEntered;
            attackRangeTrigger.ExitedTrigger -= OnAttackExited;
        }
    }

    private void OnViewEntered(Collider other)
    {
        if (other.CompareTag(GameConstants.PLAYER_TAG))
        {
            target = other.gameObject;
            isPlayerInViewRange = true;
        }
    }

    private void OnViewExited(Collider other)
    {
        if (other.CompareTag(GameConstants.PLAYER_TAG))
        {
            target = null;
            isPlayerInViewRange = false;
        }
    }

    private void OnAttackEntered(Collider other)
    {
        if (other.CompareTag(GameConstants.PLAYER_TAG)) isPlayerInAttackRange = true;
    }

    private void OnAttackExited(Collider other)
    {
        if (other.CompareTag(GameConstants.PLAYER_TAG)) isPlayerInAttackRange = false;
    }
    #endregion

    #region IDamageable Implementation
    public void TakeDamage(int amount, Vector3 direction)
    {
        if (!_currentBaseState.CanTakeDamage) return;
        
        data.CurrentKo += amount;
        
        if (KoSlider != null)
        {
            KoSlider.maxValue = data.MaxKo;
            this.UpdateSlider(KoSlider, data.CurrentKo);
        }
        
        
        context.HitDirection = direction;
        ChangeState(EnemyHit);
    }

    public Transform GetTransform() => transform;
    
    public void GettingParry()
    {
        if (_currentBaseState is not EnemyAttackState { CanBeParry: true }) return;
        ChangeState(ExposedState);
        print("Getting parry");

    }

    public void ResetKo()
    {
        data.CurrentKo = 0;
        if (KoSlider != null) KoSlider.value = 0f;
    }
    #endregion

    #region ICarryable Implementation
    public bool IsCarryable()
    {
        return _currentBaseState == KoState;
    }

    public void Carry(Transform anchor)
    {
        agent.enabled = false;
    
        rb.isKinematic = true; 
        rb.useGravity = false;

        transform.SetParent(anchor);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    
        mainCollider.enabled = false;
        isCarry = true;
    }

    public void Eject(bool isEscaping = false)
    {
        transform.SetParent(null, true);
        mainCollider.enabled = true;
    
        ApplyMovementMode(true); 

        Vector3 forceDirection = transform.forward + Vector3.up; 
        rb.AddForce(forceDirection * 5f, ForceMode.Impulse);

        isCarry = false;

        ChangeState(DropState); 
    }
    

    public bool IsCarry() => isCarry;

    #endregion
    
    public void ApplyMovementMode(bool usePhysics)
    {
        if (usePhysics)
        {
            agent.enabled = false;
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        else
        {
            if (!rb.isKinematic) 
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            rb.isKinematic = true;
            rb.useGravity = false;

            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                agent.enabled = true; 
                agent.Warp(hit.position);
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.lawnGreen;
        Vector3 start = transform.position + (Vector3.up * 0.1f);
        Gizmos.DrawLine(start, start + Vector3.down * 1.2f);
    }

}