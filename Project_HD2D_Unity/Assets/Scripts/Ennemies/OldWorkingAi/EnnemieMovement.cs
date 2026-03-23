using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnnemieMovement : MonoBehaviour
{
    #region Link
    public NavMeshAgent agent;
    
    [Header("Visual Settings")]
    public Transform visualTransform; 
    public float rotationSpeed = 10f;
    public float raycastDistance = 1.5f;
    public LayerMask groundLayer;
    #endregion
    
    void Update()
    {
        AlignVisualToGround();
    }

    private void AlignVisualToGround()
    {
        if (visualTransform == null) return;
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, raycastDistance, groundLayer))
        {
            Vector3 groundNormal = hit.normal;
            
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
            
            visualTransform.rotation = Quaternion.Slerp(visualTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    #region Core Methods (Inchangé)
    public void SetTarget(Vector3 position)
    {
        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(position);
        }
    }

    public void StopMovement()
    {
        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    public bool HasReachedDestination()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void SetSpeed(float speed) => agent.speed = speed;
    #endregion
}