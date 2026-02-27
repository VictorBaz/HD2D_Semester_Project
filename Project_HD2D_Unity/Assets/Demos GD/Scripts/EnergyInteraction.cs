using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Demos_GD.Scripts
{
    public class EnergyInteraction : MonoBehaviour
    {
        private EnergyEntryPoint currentEnergyEntryPoint;
        
        private void OnTriggerEnter(Collider other)
        {
            EnergyEntryPoint energyInterface = other.GetComponent<EnergyEntryPoint>();
            if (!energyInterface) return;
            
            currentEnergyEntryPoint = energyInterface;
        }

        private void OnTriggerExit(Collider other)
        {
            currentEnergyEntryPoint = null;
        }

        private void Update()
        {
            Gamepad gamepad = Gamepad.current;
            if (gamepad == null) return;

            if (gamepad.buttonWest.wasPressedThisFrame)
            {
                currentEnergyEntryPoint?.AddEnergy();
            }
            else if (gamepad.buttonNorth.wasPressedThisFrame)
            {
                currentEnergyEntryPoint?.RemoveEnergy();
            }
        }
    }
}