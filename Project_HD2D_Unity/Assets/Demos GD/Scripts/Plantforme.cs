using System;
using UnityEngine;

namespace Demos_GD.Scripts
{
    public class Plantforme : MonoBehaviour
    {
        [SerializeField] private RootLinker rootLinker;
        [SerializeField] private Transform[] positions;
        [SerializeField] private Transform plateforme;
        
        private void OnEnable()
        {
            rootLinker.OnEnergyChanged += Raise;
        }

        private void OnDisable()
        {
            rootLinker.OnEnergyChanged -= Raise;
        }

        private void Raise(int energy)
        {
            int clampedEnergy = Math.Clamp(energy, 0, positions.Length - 1);
            plateforme.localPosition = positions[clampedEnergy].position;
        }
    }
}