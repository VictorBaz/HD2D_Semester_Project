using UnityEngine;

public class EnnmieBrain : MonoBehaviour
{
    #region Settings
    
        public LayerMask obstacleMask;
        [SerializeField] private Trigger viewRangeTrigger;
        [SerializeField] private float eyeHeightOffset = 1.5f;
        
    #endregion

    #region Variables
    
        private EnnemieMovement movement;
        private Transform targetPlayer;
        private bool isPlayerInRange = false;
        
    #endregion


    #region UnityLifeCycle

    void Awake()
    {
        movement = GetComponent<EnnemieMovement>();
        viewRangeTrigger.EnteredTrigger += OnPlayerEntered;
        viewRangeTrigger.ExitedTrigger += OnPlayerExited;
    }

    void Update()
    {
        if (isPlayerInRange && targetPlayer != null)
        {
            if (HasLineOfSight())
            {
                movement.SetTarget(targetPlayer.position);
            }
            else
            {
                movement.StopMovement();
            }
        }
        else
        {
            movement.StopMovement();
        }
    }

    #endregion
    

    #region Raycast

    bool HasLineOfSight()
    {
        Vector3 eyePosition = transform.position + Vector3.up * eyeHeightOffset;
        Vector3 directionToPlayer = (targetPlayer.position - eyePosition).normalized;
        float distanceToPlayer = Vector3.Distance(eyePosition, targetPlayer.position);

        RaycastHit hit;
        
        if (Physics.Raycast(eyePosition, directionToPlayer, out hit, distanceToPlayer, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.CompareTag("Player"))
            {
                Debug.DrawRay(eyePosition, directionToPlayer * hit.distance, Color.green);
                return true;
            }
        }
        Debug.DrawRay(eyePosition, directionToPlayer * distanceToPlayer, Color.red);
        return false;
    }

    #endregion

    #region Trigger

    void OnPlayerEntered(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            targetPlayer = collider.transform;
            isPlayerInRange = true;
        }
    }

    void OnPlayerExited(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            isPlayerInRange = false;
            targetPlayer = null;
        }
    }

    #endregion
    
}