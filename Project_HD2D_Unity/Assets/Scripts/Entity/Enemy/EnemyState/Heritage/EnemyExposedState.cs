using UnityEngine;

public class EnemyExposedState : EnemyBaseState
{
    float timerExposed = 0;
    float timerExposedMax = 0;

    public override string Name => "Exposed";

    public override void EnterState(AiContext actx)
    {
        actx.AnimManager.SetExposed(true);
        timerExposedMax = actx.Data.ExposedTime;
    }

    public override void UpdateState(AiContext actx)
    {
        TimerExposed(actx);
    }

    public override void ExitState(AiContext actx)
    {
        actx.AnimManager.SetExposed(false);
    }

    private void TimerExposed(AiContext actx)
    {
        timerExposed += Time.deltaTime; 

        if (timerExposed >= timerExposedMax)
        {
            actx.TransitionTo(actx.Behavior.PatrolState);
            return;
        }
    }
}