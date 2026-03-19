using UnityEngine;

public class AiChase : AiState
{
    public override string Name => "Chase";

    public override void EnterState(AiContext actx) 
    {
        if (actx.Agent.isActiveAndEnabled)
        {
            actx.Agent.isStopped = false;
        }
    }

    public override void UpdateState(AiContext actx)
    {
        if (!actx.Behavior.CanSeePlayer()) 
        { 
            actx.TransitionTo(actx.Behavior.SearchState); 
            return; 
        }
        
        if (actx.IsPlayerInAttackRange) 
        {
            actx.TransitionTo(actx.Behavior.AttackState);
            return;
        }
        
        if (actx.Target != null && actx.Agent.isActiveAndEnabled)
        {
            actx.Agent.SetDestination(actx.Target.transform.position);
        }
    }

    public override void ExitState(AiContext actx) 
    {
  
    }
    
    public override bool CanAttack => true;
    public override bool CanMove => true;
    public override bool CanTakeDamage => true;
}