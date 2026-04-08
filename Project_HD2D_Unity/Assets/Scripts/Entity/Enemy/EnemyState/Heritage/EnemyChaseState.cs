using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    public override string Name => "Chase";

    public override bool CanAttack     => true;
    public override bool CanMove       => true;
    public override bool CanTakeDamage => true;

    public override void EnterState(EnemyContext actx)
    {
        actx.Manager.ApplyMovementMode(false);
        actx.ResumeAgent();
        actx.UpdateAgentSpeed(actx.Data.ChaseSpeed, actx.Data.Acceleration, actx.Data.StoppingDistance);
    }

    public override void UpdateState(EnemyContext actx)
    {
        if (!actx.Manager.CanSeePlayer())
        {
            actx.TransitionTo(actx.Manager.SearchState);
            return;
        }

        if (actx.IsPlayerInAttackRange)
        {
            actx.TransitionTo(actx.Manager.AttackState);
            return;
        }

        if (actx.Target != null)
            actx.SetDestination(actx.Target.transform.position);
    }

    public override void ExitState(EnemyContext actx) { }
}