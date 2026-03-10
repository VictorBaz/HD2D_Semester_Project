using UnityEngine;

[System.Serializable]
public class AiGoToSpawn : AiState
{
    public override void EnterState(AiBehavior core)
    {
        core.movement.SetTarget(core.spawnPosition);
    }
    public override void UpdateState(AiBehavior core)
    {
        if (core.CanSeePlayer())
        {
            core.ChangeState(core.chaseState); return;
        }

        if (core.movement.HasReachedDestination())
        {
            core.ChangeState(core.patrolState);
        }
            
    }
    public override void ExitState(AiBehavior core) { }
    
    public override string Name => "Go to spawn";
}