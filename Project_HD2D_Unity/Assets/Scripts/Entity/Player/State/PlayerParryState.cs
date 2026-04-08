using System.Collections;
using UnityEngine;

public class PlayerParryState : PlayerBaseState
{
    public override string Name               => "Parry";
    public override bool   CanMove            => false;
    public override bool   CanParry           => false;
    public override bool   IsParryWindowActive => isWindowActive;
    public bool            IsPerfectWindowActive => isPerfectWindowActive;

    private bool isWindowActive;
    private bool isPerfectWindowActive;

    private Coroutine parryRoutine;
    private Coroutine perfectParryRoutine;

    public override void EnterState(PlayerStateContext psc)
    {
        psc.AnimationManager.SetParry(true);
        psc.Controller.SetGravity(false);
        psc.Rb.linearVelocity = Vector3.zero;

        isWindowActive        = false;
        isPerfectWindowActive = false;

        if (parryRoutine != null) psc.Controller.StopCoroutine(parryRoutine);
        parryRoutine = psc.Controller.RunRoutine(ParrySequence(psc));
    }

    public override void ExitState(PlayerStateContext psc)
    {
        if (parryRoutine != null)
        {
            psc.Controller.StopCoroutine(parryRoutine);
            parryRoutine = null;
        }

        if (perfectParryRoutine != null)
        {
            psc.Controller.StopCoroutine(perfectParryRoutine);
            perfectParryRoutine = null;
        }

        isWindowActive        = false;
        isPerfectWindowActive = false;

        psc.AnimationManager.SetParry(false);
        psc.Controller.SetGravity(true);
    }

    public override void UpdateState(PlayerStateContext psc) { }

    public override void FixedUpdateState(PlayerStateContext psc) { }

    private IEnumerator ParrySequence(PlayerStateContext psc)
    {
        var   data          = psc.PlayerData;
        float animDuration  = data.ParryAnimationClip.length;

        perfectParryRoutine = psc.Controller.RunRoutine(PerfectParryRoutine(data));

        yield return new WaitForSeconds(data.ParryHitboxStartOffset);

        isWindowActive = true;
        yield return new WaitForSeconds(data.ParryActiveDuration);
        isWindowActive = false;

        float elapsed       = data.ParryHitboxStartOffset + data.ParryActiveDuration;
        float remainingTime = Mathf.Max(0, animDuration - elapsed);

        if (remainingTime > 0)
            yield return new WaitForSeconds(remainingTime);

        if (psc.StateMachine.CurrentPlayerState == this)
            DetermineState(psc);
    }

    private IEnumerator PerfectParryRoutine(PlayerDataInstance data)
    {
        isPerfectWindowActive = false;
        yield return new WaitForSeconds(data.PerfectParryStartOffset);
        isPerfectWindowActive = true;
        yield return new WaitForSeconds(data.PerfectParryDuration);
        isPerfectWindowActive = false;
    }
}