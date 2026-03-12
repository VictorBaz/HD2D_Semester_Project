using UnityEngine;

public interface ILockable
{
    Transform GetLockTransform();
    
    bool IsLockable();
    
    float GetLockPriority();

    public void SetEnergy(int energy);
    public void AddEnergy();
    public void RemoveEnergy();
}
