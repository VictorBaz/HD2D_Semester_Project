using System.Collections;
using UnityEngine;

[System.Serializable]
public class AiChase : AiState
{
    public override void EnterState(AiBehavior core) { }

    public override void UpdateState(AiBehavior core)
    {
        if (!core.CanSeePlayer()) 
        { 
            core.ChangeState(core.searchState); 
            return; 
        }
        
        if (core.isPlayerInAttackRange) 
        {
            core.ChangeState(core.attackState);
        }
        else 
        {
            core.movement.SetTarget(core.target.transform.position);
        }
    }

    public override void ExitState(AiBehavior core) { }

    public override string Name => "Chase";
}