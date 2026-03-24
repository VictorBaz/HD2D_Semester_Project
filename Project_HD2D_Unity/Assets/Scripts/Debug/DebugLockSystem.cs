using System;
using UnityEngine;

public class DebugLockSystem : MonoBehaviour
{
    [SerializeField] private LockOnSystem lockOnSystem;
    [SerializeField] private GameObject lockOnPrefab;
    
    private void Update()
    {
        
        if (lockOnSystem.IsLocked)
        {
            lockOnPrefab.SetActive(true);
            lockOnPrefab.transform.position = lockOnSystem.CurrentTarget.GetLockTransform().position + new Vector3(0,2,0);
        }
        else
        {
            lockOnPrefab.SetActive(false);
        }
    }
}
