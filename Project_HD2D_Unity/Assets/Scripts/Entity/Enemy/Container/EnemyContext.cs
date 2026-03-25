using UnityEngine;
using UnityEngine.AI;

public class EnemyContext
{
    public EnemyBehavior Behavior;
    public NavMeshAgent Agent;
    public Rigidbody Rb;
    public EnemyMovement Movement;
    
    public EnemyAnimationManager AnimManager;
    
    public GameObject Target;
    public Vector3 SpawnPosition;
    public Vector3 LastKnownPosition;
    
    public bool IsPlayerInViewRange;
    public bool IsPlayerInAttackRange;

    public EnemyDataInstance Data;
    
    public Vector3 HitDirection;
    
    public LayerMask LayerMaskEnemy;
    
    public void TransitionTo(EnemyBaseState newBaseState) => Behavior.ChangeState(newBaseState);
    
    public bool IsNavReady => Agent != null && Agent.isActiveAndEnabled && Agent.isOnNavMesh;

    public void StopAgent()
    {
        if (IsNavReady) Agent.isStopped = true;
    }

    public void ResumeAgent()
    {
        if (IsNavReady) Agent.isStopped = false;
    }

    public void SetDestination(Vector3 target)
    {
        if (IsNavReady) Agent.SetDestination(target);
    }

    public void UpdateAgentSpeed(float speed,float acceleration = -1,float stoppingDistance = -1)
    {
        if (!IsNavReady) return;
        
        Agent.speed = speed;
        
        if (acceleration != -1)  Agent.acceleration = acceleration;
        if (stoppingDistance != -1)  Agent.stoppingDistance = stoppingDistance;
    }
    
    
}