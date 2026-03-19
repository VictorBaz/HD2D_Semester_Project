using UnityEngine;

public class AiTakeDamage : AiState
{
    private float timer;

    public override string Name => "Taking Damage";

    public override void EnterState(AiContext actx)
    {
        if (actx.Agent.isActiveAndEnabled)
        {
            actx.Agent.isStopped = true;
        }

        if (actx.Behavior.KoSlider != null)
        {
            actx.Behavior.KoSlider.value += actx.Data.DamageToApply;

            if (actx.Behavior.KoSlider.value >= actx.Behavior.KoSliderMax)
            {
                actx.TransitionTo(actx.Behavior.AiKoState);
                return;
            }
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

    public override void ExitState(AiContext actx) { }
    
    public override bool CanMove => false;
    public override bool CanTakeDamage => false;
}