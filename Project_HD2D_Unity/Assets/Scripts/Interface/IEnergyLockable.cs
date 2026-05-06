public interface IEnergyLockable : ILockable
{
    void AddEnergy();
    void RemoveEnergy();
    bool IsContainingEnergy();
    bool IsAtMaximumEnergy();
    void OnLockStateChanged(bool locked);
}