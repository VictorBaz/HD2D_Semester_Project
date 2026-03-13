public interface IEnergyLockable : ILockable
{
    
    public void AddEnergy();
    public void RemoveEnergy();
    bool IsContainingEnergy();

    bool IsAtMaximumEnergy();
}