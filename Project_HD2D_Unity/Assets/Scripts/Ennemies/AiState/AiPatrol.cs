using UnityEngine;

public class AiPatrol : AiState
{
    private int currentPointIndex;

    public override string Name => "Patrol";

    public override void EnterState(AiContext actx) 
    {
        if (actx.Agent.isActiveAndEnabled)
        {
            actx.Agent.isStopped = false;
            
            if (actx.Behavior.patrolPoints.Length > 0)
            {
                actx.Agent.SetDestination(actx.Behavior.patrolPoints[currentPointIndex].position);
            }
        }
    }

    public override void UpdateState(AiContext actx)
    {
        if (actx.Behavior.CanSeePlayer())
        {
            actx.TransitionTo(actx.Behavior.ChaseState); 
            return;
        }

        if (actx.Behavior.patrolPoints.Length == 0) return;
        
        if (actx.Agent.isActiveAndEnabled && !actx.Agent.pathPending)
        {
            if (actx.Agent.remainingDistance <= actx.Agent.stoppingDistance)
            {
                currentPointIndex = (currentPointIndex + 1) % actx.Behavior.patrolPoints.Length;
                actx.Agent.SetDestination(actx.Behavior.patrolPoints[currentPointIndex].position);
            }
        }
    }

    public override void ExitState(AiContext actx) { }
}