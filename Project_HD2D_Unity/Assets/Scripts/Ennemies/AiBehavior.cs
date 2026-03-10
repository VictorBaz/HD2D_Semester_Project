using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AiBehavior : MonoBehaviour
{
    #region States Instances

        [Header("States Data")] 
        public AiStatic StaticState { get; private set; } = new AiStatic();
        public AiPatrol patrolState { get; private set; } = new AiPatrol();
        public AiChase chaseState = new AiChase();
        public AiAttack attackState = new AiAttack();
        public AiSearch searchState = new AiSearch();
        public AiGoToSpawn goToSpawnState = new AiGoToSpawn();
        public AiKO aiKoState = new AiKO();

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
        //KO thing , to move later for SRP (if needed)
        public Slider KoSlider;
        public const int KoSliderMax = 100;
        public bool isHold = false;
        
        //Name of the state
        public TMP_Text StateTxt;
        
        
        [HideInInspector] public EnnemieMovement movement;
        [HideInInspector] public GameObject target;
        [HideInInspector] public Vector3 spawnPosition;
        [HideInInspector] public Vector3 lastKnownPosition;
        [HideInInspector] public bool isPlayerInAttackRange;
        [HideInInspector] public bool isPlayerInViewRange;
        
    #endregion

    #region Unity LifeCycle
        void Awake()
        {
            movement = GetComponent<EnnemieMovement>();
            spawnPosition = transform.position;
            lastKnownPosition = spawnPosition;
            
            SubscribeEvents();
        }

        void Start()
        {
            //Ko thingy
            KoSlider.maxValue = KoSliderMax;
            
            //Name of state
            StateTxt.text = "DEBUG";
            
            //Default state 
            ChangeState(patrolState);
        }

        void OnEnable()
        {
            if (movement != null) movement.StopMovement();
            transform.position = spawnPosition;
            ChangeState(patrolState);
        }

        void Update() 
        {
            currentState?.UpdateState(this);
            KoHandlerUpdt();
            
            Debug.DrawRay(lastKnownPosition, Vector3.up * 2f, Color.yellow);
        }

        void OnDestroy() => UnsubscribeEvents();
        
    #endregion

    #region State Logic
        public void ChangeState(AiState newState)
        {
            currentState?.ExitState(this);
            currentState = newState;
            
            StateTxt.text = newState.Name;
            
            currentState.EnterState(this);
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
            if (other.CompareTag("Player")) target = other.gameObject; isPlayerInViewRange = true;
        }

        private void OnViewExited(Collider other)
        {
            if (other.CompareTag("Player")) target = null; isPlayerInViewRange = false;
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

    #region KoHandler

    public void KoHandlerUpdt()
    {

        if (currentState == aiKoState) return;

        if (KoSlider.value >= KoSliderMax)
        {
            ChangeState(aiKoState);
        }

    }

    public void KoSliderFill(int value)
    {

        KoSlider.value += value;

    }

    #endregion
}