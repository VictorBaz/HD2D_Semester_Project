using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public class EnnemieMovement : MonoBehaviour
{
    #region Variables

        private NavMeshAgent agent;
        private Rigidbody rb;
        private Vector3 lastTargetPosition;
        private bool hasDestination = false;

    #endregion

    #region UnityLifeCycle

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; 
    }

    void Update()
    {
        if (hasDestination)
        {
            agent.SetDestination(lastTargetPosition);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                StopMovement();
            }
        }
    }

    #endregion

    #region Core

    public void SetTarget(Vector3 position)
    {
        lastTargetPosition = position;
        hasDestination = true;
        agent.isStopped = false;
    }

    public void StopMovement()
    {
        if (!hasDestination) return;
        hasDestination = false;
        agent.ResetPath();
        agent.isStopped = true;
    }

    #endregion
    
}