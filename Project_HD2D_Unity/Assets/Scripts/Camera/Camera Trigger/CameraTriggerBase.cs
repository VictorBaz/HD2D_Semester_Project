using Manager;
using UnityEngine;

public abstract class CameraTriggerBase : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";
    
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        
        if (other.CompareTag(PLAYER_TAG))
        {
            hasTriggered = true;
            Trigger();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            hasTriggered = false;
        }
    }
    
    protected abstract void Trigger();
}