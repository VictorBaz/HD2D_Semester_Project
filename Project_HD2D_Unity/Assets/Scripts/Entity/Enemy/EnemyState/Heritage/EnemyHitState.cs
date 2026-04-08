using UnityEngine;

public class EnemyHitState : EnemyBaseState
{
    private float timer;

    public override string Name => "Taking Damage";

    public override bool CanMove       => false;
    public override bool CanTakeDamage => true;

    public override void EnterState(EnemyContext actx)
    {
        actx.Manager.ApplyMovementMode(true);
        actx.Rb.AddForce(actx.HitDirection * 5f, ForceMode.Impulse);
        actx.AnimManager.SetHit(true);

        actx.Data.CurrentKo += actx.Data.DamageToApply;

        if (actx.Data.IsKoFull())
        {
            actx.TransitionTo(actx.Manager.KoState);
            return;
        }

        timer = actx.Data.StunDuration;
    }

    public override void UpdateState(EnemyContext actx)
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
            actx.TransitionTo(actx.Manager.PreviousBaseState);
    }

    public override void ExitState(EnemyContext actx)
    {
        actx.AnimManager.SetHit(false);
    }
}