using System.Collections;
using Interface;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AiBehavior : MonoBehaviour, IDamageable, ICarryable
{
    #region Variables
    public AiStatic StaticState { get; private set; }
    public AiPatrol PatrolState { get; private set; }
    public AiChase ChaseState { get; private set; }
    public AiAttack AttackState { get; private set; }
    public AiSearch SearchState { get; private set; }
    public AiGoToSpawn GoToSpawnState { get; private set; }
    public AiKO AiKoState { get; private set; }
    public AiTakeDamage AiTakeDamage { get; private set; }

    [Header("Core Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Collider mainCollider;

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
    public int KoSliderMax = 100;
    public TMP_Text StateTxt;

    private AiState currentState;
    private AiContext context;
    private bool isFlying = false;

    [SerializeField] private EnemyData enemyDataRaw;
    private EnemyDataInstance data;

    [HideInInspector] public AiState previousState;
    [HideInInspector] public EnnemieMovement movement;
    [HideInInspector] public GameObject target;
    [HideInInspector] public Vector3 spawnPosition;
    [HideInInspector] public Vector3 lastKnownPosition;
    [HideInInspector] public bool isPlayerInAttackRange;
    [HideInInspector] public bool isPlayerInViewRange;
    [HideInInspector] public bool isHold = false;
    #endregion

    #region Unity Lifecycle
    void Awake()
    {
        data = enemyDataRaw.Init();
        
        StaticState = new AiStatic();
        PatrolState = new AiPatrol();
        ChaseState = new AiChase();
        AttackState = new AiAttack();
        SearchState = new AiSearch();
        GoToSpawnState = new AiGoToSpawn();
        AiKoState = new AiKO();
        AiTakeDamage = new AiTakeDamage();

        movement = GetComponent<EnnemieMovement>();
        spawnPosition = transform.position;
        lastKnownPosition = spawnPosition;
        
        SubscribeEvents();
        
        context = new AiContext
        {
            Behavior = this,
            Agent = agent,
            Rb = rb,
            Movement = movement,
            SpawnPosition = spawnPosition,
            LastKnownPosition = lastKnownPosition,
            Data = data
        };
    }

    void Start()
    {
        if (KoSlider != null) KoSlider.maxValue = KoSliderMax;
        if (StateTxt != null) StateTxt.text = "INIT";
        
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
        
        currentState?.UpdateState(context);
        
        Debug.DrawRay(lastKnownPosition, Vector3.up * 2f, Color.yellow);
    }

    void OnDestroy() => UnsubscribeEvents();
    #endregion

    #region State Logic
    public void ChangeState(AiState newState)
    {
        currentState?.ExitState(context);
        previousState = currentState;
        
        currentState = newState;
        if (StateTxt != null) StateTxt.text = newState.Name;
        
        currentState.EnterState(context);
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
        if (other.CompareTag("Player"))
        {
            target = other.gameObject;
            isPlayerInViewRange = true;
        }
    }

    private void OnViewExited(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target = null;
            isPlayerInViewRange = false;
        }
    }

    private void OnAttackEntered(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInAttackRange = true;
    }

    private void OnAttackExited(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInAttackRange = false;
    }
    #endregion

    #region IDamageable Implementation
    public void TakeDamage(int value)
    {
        data.DamageToApply = value;
        ChangeState(AiTakeDamage);
    }
    #endregion

    #region ICarryable Implementation
    public bool IsCarryable()
    {
        return currentState == AiKoState;
    }

    public void Carry(Transform anchor)
    {
        if (agent != null) agent.enabled = false;

        rb.isKinematic = true;
        rb.useGravity = false;
        
        transform.SetParent(anchor);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        mainCollider.enabled = false;
    }

    public void Eject()
    {
        Vector3 forceDirection = transform.forward; 

        transform.SetParent(null, true);

        mainCollider.enabled = true;
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce((forceDirection + Vector3.up * 0.5f) * 6f, ForceMode.Impulse);

        isFlying = true;
        StartCoroutine(LandingRoutine());
    }
    
    private IEnumerator LandingRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        bool grounded = false;
        while (!grounded)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.2f))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 0.5f, NavMesh.AllAreas))
                {
                    grounded = true;
                }
            }
            yield return new WaitForFixedUpdate();
        }

        ReactivateAI();
    }
    
    private void ReactivateAI()
    {
        isFlying = false;
        rb.isKinematic = true; 
        
        if (agent != null)
        {
            agent.enabled = true;
            agent.Warp(transform.position);
        }
    }
    #endregion
}