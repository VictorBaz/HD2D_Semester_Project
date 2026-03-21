using UnityEngine;

public class AiKO : AiState
{
    private float koTimer;

    public override string Name => "K-O";

    public override void EnterState(AiContext actx)
    {
        actx.Behavior.SetPhysicalMode(true); 
        
        actx.Rb.linearVelocity = Vector3.zero;
        
        actx.Rb.isKinematic = true; 

        actx.AnimManager.SetKO(true);
        koTimer = actx.Data.KoTime;
    }

    public override void UpdateState(AiContext actx)
    {
        koTimer -= Time.deltaTime;

        if (actx.Behavior.KoSlider != null)
        {
            actx.Behavior.KoSlider.value = 1f - (koTimer / actx.Data.KoTime);
        }

        if (koTimer <= 0)
        {
            DetermineNextState(actx);
        }
    }

    private void DetermineNextState(AiContext actx)
    {
        if (actx.Behavior.IsCarry())
        {
            actx.Behavior.Eject(true);
        }
        else
        {
            actx.TransitionTo(actx.Behavior.PatrolState);
        }
    }

    public override void ExitState(AiContext actx)
    {
        actx.AnimManager.SetKO(false);
    }

    public override bool CanAttack => false;
    public override bool CanMove => false;
    public override bool CanTakeDamage => false; 
}