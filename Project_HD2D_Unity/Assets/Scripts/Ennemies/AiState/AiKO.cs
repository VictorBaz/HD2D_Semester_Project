using UnityEngine;

public class AiKO : AiState
{
    private float koTimer;

    public override string Name => "K-O";

    public override void EnterState(AiContext actx)
    {
        //actx.Rb.linearVelocity = Vector3.zero;
        actx.Behavior.ApplyMovementMode(true); 
        actx.Rb.isKinematic = true; 

        actx.AnimManager.SetKO(true);

        if (koTimer <= 0)
        {
            koTimer = actx.Data.KoTime;
        }
    }

    public override void UpdateState(AiContext actx)
    {
        koTimer -= Time.deltaTime;

        if (actx.Behavior.KoSlider != null)
        {
            actx.Behavior.KoSlider.value = (koTimer / actx.Data.KoTime) * actx.Data.MaxKo;
        }

        if (koTimer <= 0)
        {
            actx.Data.ResetKo();
        
            DetermineNextState(actx);
        }
    }

    private void DetermineNextState(AiContext actx)
    {
        if (actx.Behavior.IsCarry())
        {
            actx.Behavior.Eject(true); 
            return; 
        }

        bool isStillInAir = !Physics.Raycast(actx.Behavior.transform.position, Vector3.down, 1.2f);

        if (isStillInAir)
        {
            actx.TransitionTo(actx.Behavior.AiDropState);
        }
        else
        {
            actx.TransitionTo(actx.Behavior.GoToSpawnState);
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