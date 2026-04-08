using UnityEngine;

public class EnemyExposedState : EnemyBaseState
{
    private float timer;

    public override string Name => "Exposed";

    public override void EnterState(EnemyContext actx)
    {
        timer = actx.Data.ExposedTime;
        actx.AnimManager.SetExposed(true);
    }

    public override void UpdateState(EnemyContext actx)
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
            actx.TransitionTo(actx.Manager.PreviousBaseState);
    }

    public override void ExitState(EnemyContext actx)
    {
        actx.AnimManager.SetExposed(false);
    }
}