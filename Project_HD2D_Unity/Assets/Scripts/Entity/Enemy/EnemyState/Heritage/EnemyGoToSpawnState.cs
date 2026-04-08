using UnityEngine;

public class EnemyGoToSpawnState : EnemyBaseState
{
    public override string Name => "Go to spawn";

    public override bool CanAttack     => true;
    public override bool CanMove       => true;
    public override bool CanTakeDamage => true;

    public override void EnterState(EnemyContext actx)
    {
        actx.Manager.ApplyMovementMode(false);
        actx.ResumeAgent();
        actx.SetDestination(actx.SpawnPosition);
    }

    public override void UpdateState(EnemyContext actx)
    {
        if (actx.Manager.CanSeePlayer())
        {
            actx.TransitionTo(actx.Manager.ChaseState);
            return;
        }

        if (actx.IsNavReady && !actx.Agent.pathPending && actx.Agent.remainingDistance <= actx.Agent.stoppingDistance)
            actx.TransitionTo(actx.Manager.PatrolState);
    }

    public override void ExitState(EnemyContext actx) { }
}