using UnityEngine;

public class EnemyBaseGoToSpawnState : EnemyBaseState
{
    public override string Name => "Go to spawn";

    public override void EnterState(AiContext actx)
    {
        actx.Behavior.ApplyMovementMode(false);
        actx.ResumeAgent();
        actx.SetDestination(actx.SpawnPosition);
    }

    public override void UpdateState(AiContext actx)
    {
        if (actx.Behavior.CanSeePlayer())
        {
            actx.TransitionTo(actx.Behavior.BaseChaseState);
            return;
        }

        if (actx.IsNavReady && !actx.Agent.pathPending && actx.Agent.remainingDistance <= actx.Agent.stoppingDistance)
        {
            actx.TransitionTo(actx.Behavior.BasePatrolState);
        }
    }

    public override void ExitState(AiContext actx) { }
    
    public override bool CanAttack => true;
    public override bool CanMove => true;
    public override bool CanTakeDamage => true;
}