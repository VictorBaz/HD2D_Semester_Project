using UnityEngine;

public class EnemyStaticState : EnemyBaseState
{
    public override string Name => "Static";

    public override bool CanAttack     => false;
    public override bool CanMove       => false;
    public override bool CanTakeDamage => false;

    public override void EnterState(EnemyContext actx)
    {
        actx.Manager.ApplyMovementMode(false);
        actx.StopAgent();
    }

    public override void UpdateState(EnemyContext actx)
    {
        if (actx.Manager.CanSeePlayer())
            actx.TransitionTo(actx.Manager.ChaseState);
    }

    public override void ExitState(EnemyContext actx) { }
}