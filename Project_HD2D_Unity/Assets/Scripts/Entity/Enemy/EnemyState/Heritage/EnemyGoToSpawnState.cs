using UnityEngine;

public class EnemyGoToSpawnState : EnemyBaseState
{
    public override string Name => "Go to spawn";

    public override void EnterState(EnemyContext actx)
    {
        actx.Behavior.ApplyMovementMode(false);
        actx.ResumeAgent();
        actx.SetDestination(actx.SpawnPosition);
    }

    public override void UpdateState(EnemyContext actx)
    {
        if (actx.Behavior.CanSeePlayer())
        {
            actx.TransitionTo(actx.Behavior.ChaseState);
            return;
        }

        if (actx.IsNavReady && !actx.Agent.pathPending && actx.Agent.remainingDistance <= actx.Agent.stoppingDistance)
        {
            actx.TransitionTo(actx.Behavior.PatrolState);
        }
    }

    public override void ExitState(EnemyContext actx) { }
    
    public override bool CanAttack => true;
    public override bool CanMove => true;
    public override bool CanTakeDamage => true;
}