using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    [Header("Visual Settings")]
    [SerializeField] private Transform visualTransform;
    [SerializeField] private float     rotationSpeed  = 10f;
    [SerializeField] private float     raycastDistance = 1.5f;
    [SerializeField] private LayerMask groundLayer;

    private void Update()
    {
        AlignVisualToGround();
    }

    private void AlignVisualToGround()
    {
        if (visualTransform == null) return;

        if (!Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer))
            return;

        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        visualTransform.rotation  = Quaternion.Slerp(visualTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public void SetTarget(Vector3 position)
    {
        if (!agent.isOnNavMesh) return;
        agent.isStopped = false;
        agent.SetDestination(position);
    }

    public void StopMovement()
    {
        if (!agent.isOnNavMesh) return;
        agent.isStopped = true;
        agent.ResetPath();
    }

    public bool HasReachedDestination()
    {
        return !agent.pathPending
               && agent.remainingDistance <= agent.stoppingDistance
               && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
    }

    public void SetSpeed(float speed) => agent.speed = speed;
}