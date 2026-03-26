using UnityEngine;

public class EnemyExposedState : EnemyBaseState
{
    float timerExposed = 0;
    float timerExposedMax = 0;

    public override string Name => "Exposed";

    public override void EnterState(EnemyContext actx)
    {
        actx.AnimManager.SetExposed(true);
        timerExposedMax = actx.Data.ExposedTime;
    }

    public override void UpdateState(EnemyContext actx)
    {
        TimerExposed(actx);
    }

    public override void ExitState(EnemyContext actx)
    {
        actx.AnimManager.SetExposed(false);
    }

    private void TimerExposed(EnemyContext actx)
    {
        timerExposed += Time.deltaTime; 

        if (timerExposed >= timerExposedMax)
        {
            actx.TransitionTo(actx.Behavior.PatrolState);
            return;
        }
    }
}