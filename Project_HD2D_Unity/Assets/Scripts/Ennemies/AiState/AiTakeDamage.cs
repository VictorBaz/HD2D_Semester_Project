using UnityEngine;

[System.Serializable]
public class AiTakeDamage : AiState
{
    public int DamageToApply;
    public float StunDuration = 0.2f;
    private float timer;
    
    public override void EnterState(AiBehavior core)
    {
        core.movement.StopMovement();
        
        core.KoSlider.value += DamageToApply;
        
        if (core.KoSlider.value >= core.KoSliderMax)
        {
            core.ChangeState(core.aiKoState);
            return;
        }
        
        timer = StunDuration;
    }

    public override void UpdateState(AiBehavior core)
    {
        timer -= Time.deltaTime;
        
        if (timer <= 0)
        {
            core.ChangeState(core.previousState);
        }
        
    }

    public override void ExitState(AiBehavior core)
    {
        
    }

    public override string Name => "Taking Damage";
}