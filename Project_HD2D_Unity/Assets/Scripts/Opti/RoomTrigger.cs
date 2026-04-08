using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public RoomActivationScript roomActivationScript;
    public GameObject otherTrigger; 
    public int roomIndex;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            roomActivationScript.ActivateRoom(roomIndex);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (otherTrigger != null)
            {
                otherTrigger.SetActive(true);
                hasTriggered = false; 
            }
            
            this.gameObject.SetActive(false);
        }
    }
}