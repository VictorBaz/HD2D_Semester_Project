using UnityEngine;

[System.Serializable]
public class AiSearch : AiState
{
    public float searchDuration = 10f;
    public float searchRadius = 5f;
    
    private float timer;
    private Vector3 searchCenter;

    public override void EnterState(AiBehavior core) 
    { 
        timer = searchDuration;
        searchCenter = core.lastKnownPosition;
        MoveToRandomPoint(core);
    } 

    public override void UpdateState(AiBehavior core)
    {
        if (core.CanSeePlayer()) 
        { 
            core.ChangeState(core.chaseState); 
            return; 
        }

        timer -= Time.deltaTime;
        
        if (core.movement.HasReachedDestination())
        {
            MoveToRandomPoint(core);
        }
        
        if (timer <= 0) 
        {
            core.ChangeState(core.goToSpawnState);
        }
    }

    private void MoveToRandomPoint(AiBehavior core)
    {
        Vector2 randomCircle = Random.insideUnitCircle * searchRadius;
        Vector3 randomPoint = searchCenter + new Vector3(randomCircle.x, 0, randomCircle.y);
        
        core.movement.SetTarget(randomPoint);
    }

    public override void ExitState(AiBehavior core) { }
    
    public override string Name => "Searching";
}