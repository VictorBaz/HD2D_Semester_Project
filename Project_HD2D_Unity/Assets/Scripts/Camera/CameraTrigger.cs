using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private CameraSettings newCameraSettings;
    
    private bool hasTriggered = false;

    private const string PLAYER_TAG = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        
        if (other.CompareTag(PLAYER_TAG))
        {
            hasTriggered = true;
            CameraEvents.TriggerCamera(newCameraSettings);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            hasTriggered = false;
        }
    }
}
