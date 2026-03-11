using UnityEngine;

public interface ILockable
{
    Transform GetLockTransform();
    bool IsLockable();
    float GetLockPriority();
    
}
