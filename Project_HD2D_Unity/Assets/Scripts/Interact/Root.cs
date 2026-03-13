using UnityEngine;

public class Root : MonoBehaviour, IEnergyLockable
{
    [SerializeField] private VATManager vatManager;
    [SerializeField] private Transform pivotPoint;

    #region IEnergyLockable

    public Transform GetLockTransform() => pivotPoint;
    public bool IsLockable() => true;
    public float GetLockPriority() => 1f;

    public bool IsContainingEnergy() => vatManager.IsContainingEnergy();
    public bool IsAtMaximumEnergy() => vatManager.IsAtMaximumEnergy();

    public void AddEnergy() => vatManager.AddEnergy();
    public void RemoveEnergy() => vatManager.RemoveEnergy();

    #endregion
}