using UnityEngine;
using UnityEngine.AI;

public class AiContext
{
    public AiBehavior Behavior;
    public NavMeshAgent Agent;
    public Rigidbody Rb;
    public EnnemieMovement Movement;
    
    public AnimManagerEnnemie AnimManager;
    
    public GameObject Target;
    public Vector3 SpawnPosition;
    public Vector3 LastKnownPosition;
    
    public bool IsPlayerInViewRange;
    public bool IsPlayerInAttackRange;

    public EnemyDataInstance Data;
    
    public void TransitionTo(AiState newState) => Behavior.ChangeState(newState);
}