public interface IEnergyLockable : ILockable
{
    public void SetEnergy(int energy);
    public void AddEnergy();
    public void RemoveEnergy();
    bool IsContainingEnergy();

    bool IsAtMaximumEnergy();
}