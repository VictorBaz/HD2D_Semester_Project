using UnityEngine;
using System.Collections.Generic;

public class LockOnSystem : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    
    [Header("Lock Settings")]
    [SerializeField] private float lockRange = 15f;
    [SerializeField] private float lockAngle = 90f; 
    [SerializeField] private LayerMask lockableLayer;
    
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;
    
    public ILockable CurrentTarget { get; private set; }
    public bool IsLocked => CurrentTarget != null;

    private List<ILockable> lockableTargets = new List<ILockable>();

    #endregion

    public void ToggleLock()
    {

        if (IsLocked)
        {
            Unlock();
        }
        else
        {
            TryLock();
        }
    }

    public void UpdateLockRotation()
    {
        if (!IsLocked) return;
        
        if (!IsTargetValid(CurrentTarget))
        {
            Unlock();
            return;
        }
        
        
        Vector3 directionToTarget = (CurrentTarget.GetLockTransform().position - playerTransform.position).normalized;
        directionToTarget.y = 0; 
        
        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            playerTransform.rotation = Quaternion.Slerp(
                playerTransform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void TryLock()
    {
        lockableTargets = FindLockableTargets();

        if (lockableTargets.Count == 0) return;

        CurrentTarget = GetBestLockableTarget(lockableTargets);
    }

    private void Unlock()
    {
        CurrentTarget = null;
    }

    private List<ILockable> FindLockableTargets()
    {
        lockableTargets.Clear();

        Collider[] colliders = Physics.OverlapSphere(playerTransform.position, lockRange, lockableLayer);

        foreach (Collider collider in colliders)
        {
            ILockable lockable = collider.GetComponent<ILockable>();

            if (lockable != null && lockable.IsLockable())
            {
                Vector3 directionToTarget = (lockable.GetLockTransform().position - playerTransform.position).normalized;
                float angleToTarget = Vector3.Angle(playerTransform.forward, directionToTarget);


                if (angleToTarget <= lockAngle)
                {
                    lockableTargets.Add(lockable);
                }
            }
        }

        return lockableTargets;
    }

    private ILockable GetBestLockableTarget(List<ILockable> targets)
{
    if (targets.Count == 0) return null;

    ILockable bestTarget = null;
    float bestScore = float.MaxValue;

    foreach (ILockable target in targets)
    {
        if (!IsTargetValid(target)) continue;
        
        float distance = Vector3.Distance(playerTransform.position, target.GetLockTransform().position);
        
        Vector3 direction = (target.GetLockTransform().position - playerTransform.position).normalized;

        float angle = Vector3.Angle(playerTransform.forward, direction);
        
        float score = distance + (angle * 0.1f);
        
        if (score < bestScore)
        {
            bestScore = score;
            bestTarget = target;
        }
    }

    return bestTarget;
}

    private bool IsTargetValid(ILockable target)
    {
        if (target == null) return false;

        if (!target.IsLockable()) return false;

        float distancePlayerToTarget = Vector3.Distance(playerTransform.position, target.GetLockTransform().position);

        if (distancePlayerToTarget > lockRange) return false;

        return true;
    }

    private bool HasLineOfSight(ILockable target)
{
    Vector3 startPos = playerTransform.position + Vector3.up; 
    Vector3 targetPos = target.GetLockTransform().position;
    
    if (Physics.Linecast(startPos, targetPos, out RaycastHit hit))
    {
        return hit.collider.GetComponent<ILockable>() == target;
    }
    
    return true; 
}

}
