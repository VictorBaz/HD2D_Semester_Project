using System.Collections.Generic;
using UnityEngine;

public class AiCore : MonoBehaviour
{
    /*
    #region Settings
    [Header("Triggers")]
    public Trigger viewRangeTrigger;
    public Trigger attackRangeTrigger;

    [Header("Detection Settings")]
    public LayerMask obstacleMask;
    public float eyeHeightOffset = 1.5f;
    
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    #endregion

    #region References
    [HideInInspector] public EnnemieMovement movement;
    [HideInInspector] public Transform targetPlayer;
    [HideInInspector] public Vector3 spawnPosition;
    [HideInInspector] public bool isPlayerInAttackRange;
    
    private AiState currentState;
    private Dictionary<System.Type, AiState> states = new Dictionary<System.Type, AiState>();
    #endregion

    #region Unity LifeCycle
    void Awake()
    {
        movement = GetComponent<EnnemieMovement>();
        spawnPosition = transform.position;
        
        InitStates();
        SubscribeEvents();
    }

    void OnEnable()
    {
        if (movement != null) movement.StopMovement();
        transform.position = spawnPosition;
        ChangeState<AiStatic>();
    }

    void Update() => currentState?.UpdateState();

    void OnDestroy() => UnsubscribeEvents();
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

    private void OnViewEntered(Collider other) { if (other.CompareTag("Player")) targetPlayer = other.transform; }
    private void OnViewExited(Collider other) { if (other.CompareTag("Player")) targetPlayer = null; }
    private void OnAttackEntered(Collider other) { if (other.CompareTag("Player")) isPlayerInAttackRange = true; }
    private void OnAttackExited(Collider other) { if (other.CompareTag("Player")) isPlayerInAttackRange = false; }
    #endregion

    #region Logic
    public bool CanSeePlayer()
    {
        if (targetPlayer == null) return false;

        Vector3 eyePos = transform.position + Vector3.up * eyeHeightOffset;
        Vector3 dir = (targetPlayer.position - eyePos).normalized;
        float dist = Vector3.Distance(eyePos, targetPlayer.position);

        if (Physics.Raycast(eyePos, dir, out RaycastHit hit, dist, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.CompareTag("Player"))
            {
                Debug.DrawRay(eyePos, dir * dist, Color.green);
                return true;
            }
        }
        Debug.DrawRay(eyePos, dir * dist, Color.red);
        return false;
    }
    #endregion

    #region State Management
    public void ChangeState<T>() where T : AiState
    {
        System.Type type = typeof(T);
        if (!states.ContainsKey(type)) return;

        currentState?.ExitState();
        currentState = states[type];
        currentState.EnterState();
    }

    private void InitStates()
    {
        states.Clear();
        foreach (var state in GetComponents<AiState>())
        {
            state.Initialize(this);
            states[state.GetType()] = state;
        }
    }
    #endregion
    */
}