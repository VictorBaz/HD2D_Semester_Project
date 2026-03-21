using UnityEngine;
using UnityEngine.AI;

public class AiDrop : AiState
{
    public override string Name => "Falling";
    private bool isGrounded = false;

    public override void EnterState(AiContext actx) 
    {
        actx.Behavior.SetPhysicalMode(true);
    
        actx.Rb.isKinematic = false; 
        actx.Rb.useGravity = true;
    
        actx.AnimManager.SetFalling(true);
    }

    public override void UpdateState(AiContext actx) 
    {
        if (!isGrounded && Physics.Raycast(actx.Behavior.transform.position, Vector3.down, out RaycastHit hit, 1.1f))
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
        actx.Rb.isKinematic = true;
        actx.Rb.linearVelocity = Vector3.zero;

        if (actx.Agent != null)
        {
            actx.Agent.enabled = true;
            actx.Agent.Warp(actx.Behavior.transform.position);
        }

        if (actx.Target != null)
            actx.TransitionTo(actx.Behavior.ChaseState);
        else
            actx.TransitionTo(actx.Behavior.PatrolState);
    }

    public override void ExitState(AiContext actx)
    {
        actx.AnimManager.SetFalling(false);
        actx.Agent.enabled = true;
    }
    
    public override bool CanAttack => false;
    public override bool CanMove => false;
    public override bool CanTakeDamage => false; 
}