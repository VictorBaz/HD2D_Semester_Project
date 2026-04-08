using UnityEngine;
using UnityEngine.AI;

public class EnemyFriendlyState : EnemyBaseState
{
    private Vector3 searchCenter;
    private float   waitTimer;

    public override string Name => "Friendly";

    public override bool CanAttack     => false;
    public override bool CanMove       => true;
    public override bool CanTakeDamage => false;

    public override void EnterState(EnemyContext actx)
    {
        actx.Manager.ApplyMovementMode(false);
        actx.ResumeAgent();

        searchCenter = actx.Manager.transform.position;
        waitTimer    = 0f;
        MoveToRandomPoint(actx);
    }

    public override void UpdateState(EnemyContext actx)
    {
        if (!actx.IsNavReady || actx.Agent.pathPending || actx.Agent.remainingDistance > actx.Agent.stoppingDistance)
            return;

        waitTimer += Time.deltaTime;

        if (waitTimer >= 3f)
        {
            MoveToRandomPoint(actx);
            waitTimer = 0f;
        }
    }

    public override void ExitState(EnemyContext actx)
    {
        if (actx.Agent.isActiveAndEnabled)
            actx.Agent.ResetPath();
    }

    private void MoveToRandomPoint(EnemyContext actx)
    {
        Vector2 randomCircle = Random.insideUnitCircle * actx.Data.SearchRadius;
        Vector3 randomPoint  = searchCenter + new Vector3(randomCircle.x, 0, randomCircle.y);

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, actx.Data.SearchRadius, NavMesh.AllAreas))
            actx.SetDestination(hit.position);
    }
}