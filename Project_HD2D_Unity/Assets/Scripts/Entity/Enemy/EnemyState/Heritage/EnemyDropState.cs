using UnityEngine;
using UnityEngine.AI;

public class EnemyDropState : EnemyBaseState
{
    private bool isGrounded;

    public override string Name => "Falling";

    public override bool CanAttack     => false;
    public override bool CanMove       => false;
    public override bool CanTakeDamage => false;

    public override void EnterState(EnemyContext actx)
    {
        isGrounded = false;
        actx.Manager.ApplyMovementMode(true);
        actx.AnimManager.SetFalling(true);
    }

    public override void UpdateState(EnemyContext actx)
    {
        if (isGrounded) return;

        if (!Physics.Raycast(actx.Manager.transform.position, Vector3.down, out RaycastHit hit, 1.2f)) return;

        if (!NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 0.5f, NavMesh.AllAreas)) return;

        isGrounded = true;
        
        LandingSequence(actx);
    }

    public override void ExitState(EnemyContext actx) 
    {
        actx.AnimManager.SetFalling(false);
    }

    private void LandingSequence(EnemyContext actx)
    {
        actx.Manager.ApplyMovementMode(false);

        bool isStillKO = actx.Manager.KoSlider != null && actx.Data.KoTime > 0;

        if (isStillKO)
            actx.TransitionTo(actx.Manager.KoState);
        else if (actx.Target != null)
            actx.TransitionTo(actx.Manager.ChaseState);
        else
            actx.TransitionTo(actx.Manager.PatrolState);
    }
}