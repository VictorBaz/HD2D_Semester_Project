using UnityEngine;

public class AiStatic : AiState
{
    public override string Name => "Static";

    public override void EnterState(AiContext actx)
    {
        if (actx.Agent.isActiveAndEnabled)
        {
            actx.Agent.isStopped = true;
        }
    }

    public override void UpdateState(AiContext actx)
    {
        if (actx.Behavior.CanSeePlayer())
        {
            actx.TransitionTo(actx.Behavior.ChaseState);
        }
    }

    public override void ExitState(AiContext actx) { }
    
    public override bool CanAttack => false;
    public override bool CanMove => false;
    public override bool CanTakeDamage => false;
}