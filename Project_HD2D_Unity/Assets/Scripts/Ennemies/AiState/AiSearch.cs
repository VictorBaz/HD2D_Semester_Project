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
        // Si on retrouve le joueur, on fonce !
        if (core.CanSeePlayer()) 
        { 
            core.ChangeState(core.chaseState); 
            return; 
        }

        timer -= Time.deltaTime;

        // Si l'IA arrive au point de recherche, on en choisit un autre
        if (core.movement.HasReachedDestination())
        {
            MoveToRandomPoint(core);
        }

        // Si le temps est écoulé, on abandonne et on rentre
        if (timer <= 0) 
        {
            core.ChangeState(core.goToSpawnState);
        }
    }

    private void MoveToRandomPoint(AiBehavior core)
    {
        // On génère un point aléatoire dans un cercle
        Vector2 randomCircle = Random.insideUnitCircle * searchRadius;
        Vector3 randomPoint = searchCenter + new Vector3(randomCircle.x, 0, randomCircle.y);
        
        core.movement.SetTarget(randomPoint);
    }

    public override void ExitState(AiBehavior core) { }
}