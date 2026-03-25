using UnityEngine;
using UnityEngine.AI;

public class EnemyFriendlyState : EnemyBaseState
{
    private Vector3 searchCenter;
    private float waitTimer;

    public override string Name => "Friendly";
    
    public override void EnterState(AiContext actx)
    {
        actx.Behavior.ApplyMovementMode(false); 
        actx.ResumeAgent();
    
        searchCenter = actx.Behavior.transform.position;
        waitTimer = 0f;
        MoveToRandomPoint(actx);
    }

    public override void UpdateState(AiContext actx)
    {
        if (!actx.Behavior.isFriendly) return;

        if (actx.IsNavReady && !actx.Agent.pathPending && actx.Agent.remainingDistance <= actx.Agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= 3.0f) 
            {
                MoveToRandomPoint(actx);
                waitTimer = 0f;
            }
        }
    }
    
    private void MoveToRandomPoint(AiContext actx)
    {
        Vector2 randomCircle = Random.insideUnitCircle * actx.Data.SearchRadius;
        Vector3 randomPoint = searchCenter + new Vector3(randomCircle.x, 0, randomCircle.y);

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, actx.Data.SearchRadius, NavMesh.AllAreas))
        {
            actx.SetDestination(hit.position);
        }
    }

    public override void ExitState(AiContext actx) 
    {
        if (actx.Agent.isActiveAndEnabled)
        {
            actx.Agent.ResetPath();
        }
    }
    
    public override bool CanAttack => false;
    public override bool CanMove => true;
    public override bool CanTakeDamage => false;
}