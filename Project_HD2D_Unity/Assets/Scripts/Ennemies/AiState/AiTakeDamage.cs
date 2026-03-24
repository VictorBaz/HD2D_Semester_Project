using UnityEngine;

public class AiTakeDamage : AiState
{
    private float timer;

    public override string Name => "Taking Damage";

    public override void EnterState(AiContext actx)
    {
        actx.Behavior.ApplyMovementMode(true);
    
        actx.Rb.AddForce(actx.HitDirection * 5f, ForceMode.Impulse);
        actx.AnimManager.SetIsHit(true);
    
        actx.Data.CurrentKo += actx.Data.DamageToApply;

        if (actx.Data.IsKoFull())
        {
            actx.TransitionTo(actx.Behavior.AiKoState);
            return;
        }

        timer = actx.Data.StunDuration;
    }

    public override void UpdateState(AiContext actx)
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            actx.TransitionTo(actx.Behavior.previousState);
        }
    }

    public override void ExitState(AiContext actx)
    {
        actx.AnimManager.SetIsHit(false);
    }
    
    public override bool CanMove => false;
    public override bool CanTakeDamage => true;
}