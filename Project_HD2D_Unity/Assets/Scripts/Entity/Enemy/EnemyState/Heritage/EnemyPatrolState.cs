using UnityEngine;

public class EnemyPatrolState : EnemyBaseState
{
    private int currentPointIndex;

    public override string Name => "Patrol";

    public override void EnterState(AiContext actx) 
    {
        actx.Behavior.ApplyMovementMode(false);
        
        actx.ResumeAgent();
        
        actx.UpdateAgentSpeed(actx.Data.PatrolSpeed,actx.Data.Acceleration,actx.Data.StoppingDistance);
    
        if (actx.Behavior.patrolPoints.Length > 0)
            actx.SetDestination(actx.Behavior.patrolPoints[currentPointIndex].position);
    }

    public override void UpdateState(AiContext actx)
    {
        actx.AnimManager.UpdateMovement(actx.Agent.speed);

        if (actx.Behavior.CanSeePlayer())
        {
            actx.TransitionTo(actx.Behavior.ChaseState); return;
        }

        if (actx.IsNavReady && !actx.Agent.pathPending)
        {
            if (actx.Agent.remainingDistance <= actx.Agent.stoppingDistance)
            {
                currentPointIndex = (currentPointIndex + 1) % actx.Behavior.patrolPoints.Length;
                actx.SetDestination(actx.Behavior.patrolPoints[currentPointIndex].position);
            }
        }
        
        
    }

    public override void ExitState(AiContext actx) { }
    
    public override bool CanAttack => true;
    public override bool CanMove => true;
    public override bool CanTakeDamage => true;
}