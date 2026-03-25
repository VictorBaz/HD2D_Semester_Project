using UnityEngine;

public class EnemyStaticState : EnemyBaseState
{
    public override string Name => "Static";

    public override void EnterState(EnemyContext actx)
    {
        actx.Behavior.ApplyMovementMode(false);
        actx.StopAgent();
    }

    public override void UpdateState(EnemyContext actx)
    {
        if (actx.Behavior.CanSeePlayer())
        {
            actx.TransitionTo(actx.Behavior.ChaseState);
        }
    }

    public override void ExitState(EnemyContext actx) { }
    
    public override bool CanAttack => false;
    public override bool CanMove => false;
    public override bool CanTakeDamage => false;
}