using UnityEngine;

public class AiGoToSpawn : AiState
{
    public override string Name => "Go to spawn";

    public override void EnterState(AiContext actx)
    {
        if (actx.Agent.isActiveAndEnabled)
        {
            actx.Agent.isStopped = false;
            actx.Agent.SetDestination(actx.SpawnPosition);
        }
    }

    public override void UpdateState(AiContext actx)
    {
        if (actx.Behavior.CanSeePlayer())
        {
            actx.TransitionTo(actx.Behavior.ChaseState);
            return;
        }

        if (actx.Agent.isActiveAndEnabled && !actx.Agent.pathPending)
        {
            if (actx.Agent.remainingDistance <= actx.Agent.stoppingDistance)
            {
                actx.TransitionTo(actx.Behavior.PatrolState);
            }
        }
    }

    public override void ExitState(AiContext actx) { }
    
    public override bool CanAttack => true;
    public override bool CanMove => true;
    public override bool CanTakeDamage => true;
}