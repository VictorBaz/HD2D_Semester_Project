using System;
using Manager;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private Vector3 newCameraPosition;
    [SerializeField] private Vector3 newCameraRotation;
    
    private void OnTriggerEnter(Collider other)
    {
        TryTriggerCamera(other);
    }

    private void TryTriggerCamera(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.TriggerCamera(newCameraPosition, newCameraRotation);
        }
    }
}
