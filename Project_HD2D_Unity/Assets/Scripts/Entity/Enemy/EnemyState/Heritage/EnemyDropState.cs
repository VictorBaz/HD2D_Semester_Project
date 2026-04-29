using UnityEngine;
using UnityEngine.AI;

public class EnemyDropState : EnemyBaseState
{
    private bool isGrounded;

    public override string Name => "Falling";

    public override bool CanAttack     => false;
    public override bool CanMove       => false;
    public override bool CanTakeDamage => false;

    private float timerReset = 0f;
    private float timerResetMax = 5f;

    public override void EnterState(EnemyContext actx)
    {
        isGrounded = false;
        actx.Manager.ApplyMovementMode(true);
        actx.AnimManager.SetFalling(true);
        actx.AnimManager.ToggleRepulsiveCollider(true);
        timerReset = 0f;
    }

    public override void UpdateState(EnemyContext actx)
    {
        TimerResetEnemy(actx);
        
        if (isGrounded) return;

        if (Physics.Raycast(
                actx.Manager.transform.position,
                Vector3.down,
                out RaycastHit hit,
                actx.Data.GroundCheckDistance,actx.GroundLayerMask))
        {
            if (NavMesh.SamplePosition(
                    hit.point,
                    out NavMeshHit navHit,
                    actx.Data.NavmeshCheckDistance,
                    NavMesh.AllAreas))
            {
                isGrounded = true;
        
                LandingSequence(actx);
            }
        }
    }

    public override void ExitState(EnemyContext actx) 
    {
        actx.AnimManager.SetFalling(false);
    }

    private void LandingSequence(EnemyContext actx)
    {

        bool isStillKO = actx.Manager.KoSlider != null && actx.Data.KoTime > 0;

        if (isStillKO)
        {
            actx.TransitionTo(actx.Manager.KoState);
        }
        else if (actx.Target != null)
        {
            actx.TransitionTo(actx.Manager.ChaseState);
            actx.VfxManager.StopKoVfx();
        }
        else
        {
            actx.TransitionTo(actx.Manager.PatrolState);
            actx.VfxManager.StopKoVfx();
        }
    }

    private void TimerResetEnemy(EnemyContext actx)
    {
        if (timerReset >= timerResetMax)
        {
            actx.Manager.ResetEnemy();
            timerReset = 0f;
        }
        else
        {
            timerReset += Time.deltaTime;
        }
    }
}