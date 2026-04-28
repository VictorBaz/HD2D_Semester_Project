using UnityEngine;
using UnityEngine.AI;

public class EnemySearchState : EnemyBaseState
{
    private float   timer;
    private Vector3 searchCenter;

    public override string Name => "Searching";

    public override bool CanAttack     => true;
    public override bool CanMove       => true;
    public override bool CanTakeDamage => true;

    public override void EnterState(EnemyContext actx)
    {
        timer        = actx.Data.SearchDuration;
        searchCenter = actx.LastKnownPosition;

        actx.Manager.ApplyMovementMode(false);
        actx.ResumeAgent();
        MoveToRandomPoint(actx);
        actx.AnimManager.UpdateMovement(GameConstants.ANIM_MAGNITUDE_WALK);
        actx.VfxManager.PlayDust(true);
        actx.AnimManager.ToggleRepulsiveCollider(true);
    }

    public override void UpdateState(EnemyContext actx)
    {
        if (actx.Manager.CanSeePlayer())
        {
            actx.TransitionTo(actx.Manager.ChaseState);
            return;
        }

        timer -= Time.deltaTime;

        if (actx.IsNavReady && !actx.Agent.pathPending && actx.Agent.remainingDistance <= actx.Agent.stoppingDistance)
            MoveToRandomPoint(actx);

        if (timer <= 0)
            actx.TransitionTo(actx.Manager.GoToSpawnState);
    }

    public override void ExitState(EnemyContext actx)
    {
        actx.VfxManager.PlayDust(false);
    }

    private void MoveToRandomPoint(EnemyContext actx)
    {
        if (!actx.IsNavReady) return;
            
        Vector2 randomCircle = Random.insideUnitCircle * actx.Data.SearchRadius;
        Vector3 randomPoint  = searchCenter + new Vector3(randomCircle.x, 0, randomCircle.y);

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, actx.Data.SearchRadius, NavMesh.AllAreas))
            actx.Agent.SetDestination(hit.position);
    }
}