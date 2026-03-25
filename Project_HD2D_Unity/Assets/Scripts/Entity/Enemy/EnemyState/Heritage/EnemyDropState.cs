using UnityEngine;
using UnityEngine.AI;

public class EnemyDropState : EnemyBaseState
{
    public override string Name => "Falling";
    private bool isGrounded = false;
    
    public override bool CanAttack => false;
    public override bool CanMove => false;
    public override bool CanTakeDamage => false; 

    public override void EnterState(AiContext actx) 
    {
        isGrounded = false;
        actx.Behavior.ApplyMovementMode(true);
        actx.AnimManager.SetFalling(true);
    }

    public override void UpdateState(AiContext actx) 
    {
        if (!isGrounded && Physics.Raycast(actx.Behavior.transform.position, Vector3.down, out RaycastHit hit, 1.2f))
        {
            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 0.5f, NavMesh.AllAreas))
            {
                isGrounded = true;
                LandingSequence(actx);
            }
        }
    }

    private void LandingSequence(AiContext actx)
    {
        actx.Behavior.ApplyMovementMode(false); 

        bool isStillKO = actx.Behavior.KoSlider != null && actx.Behavior.KoSlider.value > 0;

        if (isStillKO)
        {
            actx.TransitionTo(actx.Behavior.KoState);
        }
        else 
        {
            if (actx.Target != null)
                actx.TransitionTo(actx.Behavior.ChaseState);
            else
                actx.TransitionTo(actx.Behavior.PatrolState);
        }
    }

    public override void ExitState(AiContext actx)
    {
        actx.AnimManager.SetFalling(false);
    }
}