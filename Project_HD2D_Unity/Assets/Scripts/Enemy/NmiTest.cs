using System;
using UnityEngine;

public class NmiTest : MonoBehaviour, ILockable
{
    [SerializeField] private Transform lockPoint;
    
    [SerializeField] private GameObject visualsLock;
    
    private bool isDead = false;
    
    public Transform GetLockTransform()
    {
        return lockPoint != null ? lockPoint : transform;
    }
    
    public bool IsLockable()
    {
        return !isDead;
    }
    
    public float GetLockPriority()
    {
        return 1f; 
    }

    public void SetEnergy(int energy)
    {
        
    }

    public void AddEnergy()
    {
     
    }

    public void RemoveEnergy()
    {
    
    }
}
