using UnityEngine;

public class EnemyKoState : EnemyBaseState
{
    public override string Name => "K-O";

    public override bool CanAttack     => false;
    public override bool CanMove       => false;
    public override bool CanTakeDamage => false;

    private bool hasTransitionedToLoop;

    public override void EnterState(EnemyContext actx)
    {
        actx.Manager.ApplyMovementMode(true);
        actx.Rb.isKinematic    = true;
        hasTransitionedToLoop  = false;

        if (actx.Data.KoTime <= 0)
        {
            actx.Data.KoTime = actx.Data.KoTimeMax;
            actx.AnimManager.HandleKo(true, false);
            actx.VfxManager.SetKoVfx();
        }
        else
        {
            actx.AnimManager.HandleKo(true, true);
            hasTransitionedToLoop = true;
        }
        
        
    }

    public override void UpdateState(EnemyContext actx)
    {
        actx.Data.KoTime -= Time.deltaTime;

        if (actx.Manager.KoSlider != null)
            actx.Manager.KoSlider.value = (actx.Data.KoTime / actx.Data.KoTimeMax) * actx.Data.MaxKo;

        if (!hasTransitionedToLoop)
        {
            var stateInfo = actx.AnimManager.GetCurrentState(0);

            if (stateInfo.normalizedTime >= 0.7f && !actx.AnimManager.Animator.IsInTransition(0))
            {
                actx.AnimManager.HandleKo(true, true);
                hasTransitionedToLoop = true;
            }
        }

        if (actx.Data.IsKoTimerEmpty())
        {
            actx.Data.ResetKo();
            DetermineNextState(actx);
            actx.VfxManager.StopKoVfx();
        }
    }

    public override void ExitState(EnemyContext actx)
    {
        actx.AnimManager.HandleKo(false, false);
    }

    private void DetermineNextState(EnemyContext actx)
    {
        if (actx.Manager.IsCarry())
        {
            actx.Manager.Eject(true);
            return;
        }
        
        if (!actx.Manager.IsGrounded(actx.Data.GroundDetectionDistance,actx.Data.NavMeshSampleMargin))
            actx.TransitionTo(actx.Manager.DropState);
        else
        {
            actx.Manager.RecoverPhase();
            actx.TransitionTo(actx.Manager.GoToSpawnState);
        }
    }
}