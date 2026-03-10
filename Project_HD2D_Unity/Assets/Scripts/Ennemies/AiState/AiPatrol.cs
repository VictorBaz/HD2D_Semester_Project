using UnityEngine;

[System.Serializable]
public class AiPatrol : AiState
{
    private int currentPointIndex;

    public override void EnterState(AiBehavior core) 
    {
        if (core.patrolPoints.Length > 0)
        {
            core.movement.SetTarget(core.patrolPoints[currentPointIndex].position);
        }
    }

    public override void UpdateState(AiBehavior core)
    {
        if (core.CanSeePlayer())
        {
            core.ChangeState(core.chaseState); 
            return;
        }

        if (core.patrolPoints.Length == 0) return;
        
        if (core.movement.HasReachedDestination())
        {
            currentPointIndex = (currentPointIndex + 1) % core.patrolPoints.Length;
            core.movement.SetTarget(core.patrolPoints[currentPointIndex].position);
        }
    }
    public override void ExitState(AiBehavior core) { }
    
    public override string Name => "Patrol";
}