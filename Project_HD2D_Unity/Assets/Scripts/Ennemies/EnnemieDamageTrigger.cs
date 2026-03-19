using UnityEngine;

public class EnemyDamageTrigger : MonoBehaviour
{
    public int Damage = 2;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            /*
            PlayerDataInstance playerDataInstance = other.GetComponent<PlayerDataInstance>();
        
            if (playerDataInstance != null)
            {
                playerDataInstance.Life -= Damage;
            }
            */
            
            Debug.Log("Ouchh...");
        }
        
    }
}