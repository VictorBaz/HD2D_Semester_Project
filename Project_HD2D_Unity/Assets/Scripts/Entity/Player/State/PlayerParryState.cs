using System.Collections;
using UnityEngine;

public class PlayerParryState : PlayerBaseState
{
    public override string Name              => "Parry";
    public override bool   CanMove          => false;
    public override bool   CanParry         => false;
    public override bool   IsParryWindowActive => isWindowActive;

    private bool      isWindowActive;
    private Coroutine parryRoutine;

    public override void EnterState(PlayerStateContext psc)
    {
        psc.AnimationManager.SetParry(true);
        psc.Controller.SetGravity(false);
        psc.Rb.linearVelocity = Vector3.zero;

        isWindowActive = false;
        parryRoutine   = psc.Controller.RunRoutine(ParrySequence(psc));
    }

    public override void ExitState(PlayerStateContext psc)
    {
        if (parryRoutine != null)
        {
            psc.Controller.StopCoroutine(parryRoutine);
            parryRoutine = null;
        }

        isWindowActive = false;
        psc.AnimationManager.SetParry(false);
        psc.Controller.SetGravity(true);
    }

    public override void UpdateState(PlayerStateContext psc)
    {
        HandleAnimation(psc);
    }

    public override void FixedUpdateState(PlayerStateContext psc) { }

    private IEnumerator ParrySequence(PlayerStateContext psc)
    {
        float animDuration = psc.PlayerData.ParryAnimationClip.length;

        yield return new WaitForSeconds(psc.PlayerData.ParryHitboxStartOffset);

        isWindowActive = true;

        yield return new WaitForSeconds(psc.PlayerData.ParryActiveDuration);

        isWindowActive = false;

        float remainingTime = animDuration -
                              (psc.PlayerData.ParryHitboxStartOffset + psc.PlayerData.ParryActiveDuration);

        if (remainingTime > 0.01f)
            yield return new WaitForSeconds(remainingTime);

        if (psc.StateMachine.CurrentPlayerState == this)
            DetermineState(psc);
    }
}