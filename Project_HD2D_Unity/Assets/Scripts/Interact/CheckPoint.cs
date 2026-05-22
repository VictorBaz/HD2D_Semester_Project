using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Position locale de réapparition. Déplacez la sphère jaune dans la scène.")]
    [SerializeField] private Vector3 spawnOffset = Vector3.up;
    
    public Vector3 SpawnPosition => transform.TransformPoint(spawnOffset);

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag(GameConstants.PLAYER_TAG))
        {
            ActivateCheckpoint();
        }           
    }

    private void ActivateCheckpoint()
    {
        GameplayEvents.TriggerCheckpoint(SpawnPosition);
    }

    private void OnDrawGizmos()
    {
        Vector3 worldSpawn = SpawnPosition;
        Gizmos.DrawSphere(worldSpawn, 0.2f);
        Gizmos.DrawLine(transform.position, worldSpawn);
        
    }
}