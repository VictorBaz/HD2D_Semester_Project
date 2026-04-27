using UnityEngine;

public class EnemyPatrolState : EnemyBaseState
{
    private int currentPointIndex;

    public override string Name => "Patrol";

    public override bool CanAttack     => true;
    public override bool CanMove       => true;
    public override bool CanTakeDamage => true;

    public override void EnterState(EnemyContext actx)
    {
        actx.Manager.ApplyMovementMode(false);
        actx.ResumeAgent();
        actx.UpdateAgentSpeed(actx.Data.PatrolSpeed, actx.Data.Acceleration, actx.Data.StoppingDistance);

        if (actx.Manager.patrolPoints.Length > 0)
            actx.SetDestination(actx.Manager.patrolPoints[currentPointIndex].position);
        
        actx.VfxManager.PlayDust(true);
    }

    public override void UpdateState(EnemyContext actx)
    {
        UpdateMovementAnimation(actx);
        
        if (actx.Manager.CanSeePlayer())
        {
            actx.TransitionTo(actx.Manager.ChaseState);
            return;
        }

        if (actx.Manager.patrolPoints.Length <= 0) return;
        
        if (actx.IsNavReady && !actx.Agent.pathPending && actx.Agent.remainingDistance <= actx.Agent.stoppingDistance)
        {
            currentPointIndex = (currentPointIndex + 1) % actx.Manager.patrolPoints.Length;
            actx.SetDestination(actx.Manager.patrolPoints[currentPointIndex].position);
        }
    }

    public override void ExitState(EnemyContext actx)
    {
        actx.VfxManager.PlayDust(false);
    }

    private void UpdateMovementAnimation(EnemyContext actx)
    {
        if (actx.Manager.patrolPoints.Length > 0) //then nonstatic behavior
        {
            actx.AnimManager.UpdateMovement(GameConstants.ANIM_MAGNITUDE_WALK);
        }
        else
        {
            actx.AnimManager.UpdateMovement(GameConstants.ANIM_MAGNITUDE_IDLE);
        }
    }
}