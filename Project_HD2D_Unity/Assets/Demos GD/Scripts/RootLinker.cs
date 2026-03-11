using System;
using UnityEngine;

namespace Demos_GD.Scripts
{
    public class RootLinker : MonoBehaviour
    {
        [SerializeField] private int energyLevel;

        public event Action<int> OnEnergyChanged;
        public void ChangeEnergyLevel(int amount)
        {
            energyLevel += amount;
            Debug.Log($"Energy level of {name}: {energyLevel}");
            OnEnergyChanged?.Invoke(energyLevel);
        }
    }
}
