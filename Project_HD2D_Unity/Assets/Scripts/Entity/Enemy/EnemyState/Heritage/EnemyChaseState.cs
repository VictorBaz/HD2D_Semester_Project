using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    public override string Name => "Chase";

    public override void EnterState(AiContext actx) 
    {
        actx.Behavior.ApplyMovementMode(false); 
        actx.ResumeAgent();
        
        actx.UpdateAgentSpeed(actx.Data.ChaseSpeed,actx.Data.Acceleration,actx.Data.StoppingDistance);
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
    
        if (actx.Target != null)
        {
            actx.SetDestination(actx.Target.transform.position);
        }
    }

    public override void ExitState(AiContext actx) 
    {
  
    }
    
    public override bool CanAttack => true;
    public override bool CanMove => true;
    public override bool CanTakeDamage => true;
}