using UnityEngine;
using UnityEngine.AI;

public class EnemySearchState : EnemyBaseState
{
    private float timer;
    private Vector3 searchCenter;

    public override string Name => "Searching";

    public override void EnterState(EnemyContext actx) 
    { 
        timer = actx.Data.SearchDuration;
        searchCenter = actx.LastKnownPosition;
    
        actx.Behavior.ApplyMovementMode(false);
        actx.ResumeAgent();
        MoveToRandomPoint(actx);
    } 

    public override void UpdateState(EnemyContext actx)
    {
        if (actx.Behavior.CanSeePlayer()) 
        { 
            actx.TransitionTo(actx.Behavior.ChaseState); 
            return; 
        }

        timer -= Time.deltaTime;
    
        if (actx.IsNavReady && !actx.Agent.pathPending && actx.Agent.remainingDistance <= actx.Agent.stoppingDistance)
        {
            MoveToRandomPoint(actx);
        }
    
        if (timer <= 0) 
        {
            actx.TransitionTo(actx.Behavior.GoToSpawnState);
        }
    }

    private void MoveToRandomPoint(EnemyContext actx)
    {
        Vector2 randomCircle = Random.insideUnitCircle * actx.Data.SearchRadius;
        Vector3 randomPoint = searchCenter + new Vector3(randomCircle.x, 0, randomCircle.y);

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, actx.Data.SearchRadius, NavMesh.AllAreas))
        {
            actx.Agent.SetDestination(hit.position);
        }
    }

    public override void ExitState(EnemyContext actx) { }
    
    public override bool CanAttack => true;
    public override bool CanMove => true;
    public override bool CanTakeDamage => true;
}