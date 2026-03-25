using UnityEngine;

public class EnemyStaticState : EnemyBaseState
{
    public override string Name => "Static";

    public override void EnterState(AiContext actx)
    {
        actx.Behavior.ApplyMovementMode(false);
        actx.StopAgent();
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