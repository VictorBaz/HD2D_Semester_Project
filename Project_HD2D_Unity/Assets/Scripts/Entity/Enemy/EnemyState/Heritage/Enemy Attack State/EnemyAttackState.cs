using System.Collections;
using UnityEngine;

public abstract class EnemyAttackState : EnemyBaseState
{
    protected Coroutine attackRoutine;
    protected bool isCooldown;
    protected bool isAnticipationTime;
    

    public override string Name => "Attacking";
    public override bool CanMove => attackRoutine != null;

    public override void EnterState(EnemyContext actx)
    {
        actx.StopAgent();
        
        actx.Manager.ApplyMovementMode(false);

        attackRoutine = null;
        isCooldown = false;
        CanBeParry = false;
        isAnticipationTime = false;
    }

    public override void UpdateState(EnemyContext actx)
    {
        if (!actx.Manager.IsGrounded(actx.Data.GroundDetectionDistance,actx.Data.NavMeshSampleMargin)) return;
        
        if (actx.Target == null)
        {
            actx.TransitionTo(actx.Manager.SearchState);
            return;
        }

        if (isCooldown || attackRoutine == null || isAnticipationTime)
            RotateTowardsTarget(actx);

        if (attackRoutine != null || isCooldown) return;

        if (!actx.IsPlayerInAttackRange)
        {
            actx.TransitionTo(actx.Manager.ChaseState);
            return;
        }

        attackRoutine = actx.Manager.StartCoroutine(AttackSequence(actx));
    }

    protected abstract IEnumerator AttackSequence(EnemyContext actx);

    protected void RotateTowardsTarget(EnemyContext actx)
    {
        if (actx.Target == null) return;
        Vector3 lookDir = (actx.Target.transform.position - actx.Manager.transform.position).normalized;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
        {
            actx.Manager.transform.rotation = Quaternion.Slerp(
                actx.Manager.transform.rotation, 
                Quaternion.LookRotation(lookDir), 
                Time.deltaTime * actx.Data.RotationSpeed);
        }
    }

    public override void ExitState(EnemyContext actx)
    {
        if (attackRoutine != null) actx.Manager.StopCoroutine(attackRoutine);
    }
}