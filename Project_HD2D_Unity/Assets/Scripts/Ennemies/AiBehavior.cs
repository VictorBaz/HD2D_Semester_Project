using UnityEngine;

public class AiBehavior : MonoBehaviour
{
    #region States Instances
    [Header("States Data")]
    public AiStatic staticState = new AiStatic();
    public AiPatrol patrolState = new AiPatrol();
    public AiChase chaseState = new AiChase();
    public AiAttack attackState = new AiAttack();
    public AiSearch searchState = new AiSearch();
    public AiGoToSpawn goToSpawnState = new AiGoToSpawn();

    private AiState currentState;
    #endregion

    #region Settings & Triggers
    [Header("Triggers")]
    public Trigger viewRangeTrigger;
    public Trigger attackRangeTrigger;

    [Header("Detection")]
    public LayerMask obstacleMask;
    public float eyeHeightOffset = 1.5f;
    
    [Header("Patrol")]
    public Transform[] patrolPoints;
    #endregion

    #region References
    [HideInInspector] public EnnemieMovement movement;
    [HideInInspector] public GameObject targetPlayer;
    [HideInInspector] public Vector3 spawnPosition;
    [HideInInspector] public Vector3 lastKnownPosition;
    [HideInInspector] public bool isPlayerInAttackRange;
    #endregion

    #region Unity LifeCycle
    void Awake()
    {
        movement = GetComponent<EnnemieMovement>();
        spawnPosition = transform.position;
        lastKnownPosition = spawnPosition;
        
        SubscribeEvents();
    }

    void Start() => ChangeState(patrolState);

    void OnEnable()
    {
        if (movement != null) movement.StopMovement();
        transform.position = spawnPosition;
        ChangeState(patrolState);
    }

    void Update() 
    {
        currentState?.UpdateState(this);
        
        Debug.DrawRay(lastKnownPosition, Vector3.up * 2f, Color.yellow);
    }

    void OnDestroy() => UnsubscribeEvents();
    #endregion

    #region State Logic
    public void ChangeState(AiState newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }
    #endregion

    #region Detection Logic
    public bool CanSeePlayer()
    {
        if (targetPlayer == null) return false;

        Vector3 eyePos = transform.position + Vector3.up * eyeHeightOffset;
        Vector3 dir = (targetPlayer.transform.position - eyePos).normalized;
        float dist = Vector3.Distance(eyePos, targetPlayer.transform.position);

        if (Physics.Raycast(eyePos, dir, out RaycastHit hit, dist, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.CompareTag("Player")) 
            {
                lastKnownPosition = targetPlayer.transform.position;
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

    private void OnViewEntered(Collider other) { if (other.CompareTag("Player")) targetPlayer = other.gameObject; }
    private void OnViewExited(Collider other) { if (other.CompareTag("Player")) targetPlayer = null; }
    private void OnAttackEntered(Collider other) { if (other.CompareTag("Player")) isPlayerInAttackRange = true; }
    private void OnAttackExited(Collider other) { if (other.CompareTag("Player")) isPlayerInAttackRange = false; }
    #endregion
}