using UnityEngine;
using UnityEngine.AI;

public class AiSearch : AiState
{
    private float timer;
    private Vector3 searchCenter;

    public override string Name => "Searching";

    public override void EnterState(AiContext actx) 
    { 
        timer = actx.Data.SearchDuration;
        searchCenter = actx.LastKnownPosition;
        
        if (actx.Agent.isActiveAndEnabled)
        {
            actx.Agent.isStopped = false;
            MoveToRandomPoint(actx);
        }
    } 

    public override void UpdateState(AiContext actx)
    {
        if (actx.Behavior.CanSeePlayer()) 
        { 
            actx.TransitionTo(actx.Behavior.ChaseState); 
            return; 
        }

        timer -= Time.deltaTime;
        
        if (actx.Agent.isActiveAndEnabled && !actx.Agent.pathPending)
        {
            if (actx.Agent.remainingDistance <= actx.Agent.stoppingDistance)
            {
                MoveToRandomPoint(actx);
            }
        }
        
        if (timer <= 0) 
        {
            actx.TransitionTo(actx.Behavior.GoToSpawnState);
        }
    }

    private void MoveToRandomPoint(AiContext actx)
    {
        Vector2 randomCircle = Random.insideUnitCircle * actx.Data.SearchRadius;
        Vector3 randomPoint = searchCenter + new Vector3(randomCircle.x, 0, randomCircle.y);

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, actx.Data.SearchRadius, NavMesh.AllAreas))
        {
            actx.Agent.SetDestination(hit.position);
        }
    }

    public override void ExitState(AiContext actx) { }
    
    public override bool CanAttack => true;
    public override bool CanMove => true;
    public override bool CanTakeDamage => true;
}