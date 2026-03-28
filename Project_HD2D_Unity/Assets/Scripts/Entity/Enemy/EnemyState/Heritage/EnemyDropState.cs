using UnityEngine;
using UnityEngine.AI;

public class EnemyDropState : EnemyBaseState
{
    public override string Name => "Falling";
    private bool isGrounded = false;
    
    public override bool CanAttack => false;
    public override bool CanMove => false;
    public override bool CanTakeDamage => false; 

    public override void EnterState(EnemyContext actx) 
    {
        isGrounded = false;
        actx.Manager.ApplyMovementMode(true);
        actx.AnimManager.SetFalling(true);
    }

    public override void UpdateState(EnemyContext actx) 
    {
        if (!isGrounded && Physics.Raycast(actx.Manager.transform.position, Vector3.down, out RaycastHit hit, 1.2f))
        {
            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 0.5f, NavMesh.AllAreas))
            {
                isGrounded = true;
                LandingSequence(actx);
            }
        }
    }

    private void LandingSequence(EnemyContext actx)
    {
        actx.Manager.ApplyMovementMode(false); 

        bool isStillKO = actx.Manager.KoSlider != null && actx.Manager.KoSlider.value > 0;

        if (isStillKO)
        {
            actx.TransitionTo(actx.Manager.KoState);
        }
        else 
        {
            if (actx.Target != null)
                actx.TransitionTo(actx.Manager.ChaseState);
            else
                actx.TransitionTo(actx.Manager.PatrolState);
        }
    }

    public override void ExitState(EnemyContext actx)
    {
        actx.AnimManager.SetFalling(false);
    }
}