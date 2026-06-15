using Script.Manager;
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

        if (actx.HitDirection == Vector3.zero)
        {
            actx.Rb.linearVelocity= Vector3.zero;
        }
        else
        {
            actx.Rb.AddForce(actx.HitDirection * 5f, ForceMode.VelocityChange);
        }
        
        actx.AnimManager.SetHit(true);

        if (actx.Data.IsKoFull())
        {
            actx.TransitionTo(actx.Manager.KoState);
            if (SoundManager.Instance)SoundManager.Instance.PlaySfx(SoundType.Enemy_Ko_Full);
            return;
        }

        timer = actx.Data.StunDuration;
        actx.VfxManager.PlayHitVfx();
        actx.AnimManager.ToggleRepulsiveCollider(true);
    }

    public override void UpdateState(EnemyContext actx)
    {
        timer -= Time.deltaTime;

        if (!(timer <= 0)) return;
        
        if (actx.Manager.PreviousBaseState is EnemyExposedState)
        {
            actx.TransitionTo(actx.Manager.SearchState);
            return;
        }
            
        actx.TransitionTo(actx.Manager.PreviousBaseState);
    }

    public override void ExitState(EnemyContext actx)
    {
        actx.AnimManager.SetHit(false);
    }
}