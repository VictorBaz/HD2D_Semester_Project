using System;
using System.Collections;
using UnityEngine;

namespace Demos_GD.Scripts
{
    public class Plantforme : MonoBehaviour
    {
        private const float TravelTime = 1f;
        
        [SerializeField] private RootLinker rootLinker;
        [SerializeField] private Transform[] positions;
        [SerializeField] private Transform plateforme;
        
        private Coroutine travelRoutine;
        
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

            if (travelRoutine != null) StopCoroutine(travelRoutine);
            travelRoutine = StartCoroutine(TravelCoroutine(positions[clampedEnergy].position));
        }

        private IEnumerator TravelCoroutine(Vector3 targetPosition)
        {
            Vector3 startPosition = plateforme.position;
            
            float timeElapsed = 0f;
            while (timeElapsed < TravelTime)
            {
                timeElapsed += Time.deltaTime;
                
                plateforme.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / TravelTime);
                yield return null;
            }
            
            plateforme.position = targetPosition;
        }
    }
}