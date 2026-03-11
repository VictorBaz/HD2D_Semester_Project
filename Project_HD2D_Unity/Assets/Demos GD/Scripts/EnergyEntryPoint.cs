using System;
using UnityEngine;

namespace Demos_GD.Scripts
{
    public class EnergyEntryPoint : MonoBehaviour
    {
        [SerializeField] private RootLinker rootLinker;

        private void OnTriggerEnter(Collider other)
        {
            ProjectileBase projectile = other.GetComponent<ProjectileBase>();
            if (!projectile) return;
            
            AddEnergy();
        }

        public void AddEnergy()
        {
            rootLinker.ChangeEnergyLevel(1);
        }

        public void RemoveEnergy()
        {
            rootLinker.ChangeEnergyLevel(-1);
        }
    }
}
