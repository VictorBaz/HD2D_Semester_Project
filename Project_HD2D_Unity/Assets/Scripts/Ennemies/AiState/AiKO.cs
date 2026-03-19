using System.Collections;
using UnityEngine;

public class AiKO : AiState
{
    private Coroutine KoRoutine;

    public override string Name => "K-O";

    public override void EnterState(AiContext actx)
    {
        if (actx.Agent.isActiveAndEnabled)
        {
            actx.Agent.isStopped = true;
        }

        if (actx.Behavior.KoSlider != null)
        {
            actx.Behavior.KoSlider.value = 0;
        }

        KoRoutine = actx.Behavior.StartCoroutine(KoMoment(actx));
    }

    public override void UpdateState(AiContext actx) { }

    public override void ExitState(AiContext actx)
    {
        if (KoRoutine != null)
        {
            actx.Behavior.StopCoroutine(KoRoutine);
            KoRoutine = null;
        }
    }

    private IEnumerator KoMoment(AiContext actx)
    {
        yield return new WaitForSeconds(actx.Data.KoTime);

        if (actx.IsPlayerInAttackRange && actx.Target != null)
        {
            actx.TransitionTo(actx.Behavior.AttackState);
        }
        else if (actx.IsPlayerInViewRange && actx.Target != null)
        {
            actx.TransitionTo(actx.Behavior.ChaseState);
        }
        else
        {
            actx.TransitionTo(actx.Behavior.GoToSpawnState);
        }
    }
    
    public override bool CanAttack => false;
    public override bool CanMove => false;
    public override bool CanTakeDamage => false;
}