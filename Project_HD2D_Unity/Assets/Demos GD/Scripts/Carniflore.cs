using System;
using System.Collections;
using UnityEngine;

namespace Demos_GD.Scripts
{
    public class Carniflore : MonoBehaviour
    {
        private const float ChangeTime = 1f;
        
        [SerializeField] private RootLinker rootLinker;
        [SerializeField] private float[] lengths;

        private Coroutine changeCoroutine;
        
        private void OnEnable()
        {
            rootLinker.OnEnergyChanged += Change;
        }

        private void OnDisable()
        {
            rootLinker.OnEnergyChanged -= Change;
        }

        private void Change(int energyLevel)
        {
            int clampedEnergy = Mathf.Clamp(energyLevel, 0, lengths.Length - 1);
            
            if (changeCoroutine != null) StopCoroutine(changeCoroutine);
            changeCoroutine = StartCoroutine(ChangeCoroutine(lengths[clampedEnergy]));
        }

        private IEnumerator ChangeCoroutine(float targetLenght)
        {
            float startLenght = transform.localScale.x;
            
            float timeElapsed = 0f;
            while (timeElapsed < ChangeTime)
            {
                timeElapsed += Time.deltaTime;
                
                transform.localScale = new Vector3(Mathf.Lerp(startLenght, targetLenght, timeElapsed / ChangeTime),
                    transform.localScale.y, transform.localScale.z);
                yield return null;
            }
            
            transform.localScale = new Vector3(targetLenght, transform.localScale.y, transform.localScale.z);
        }
    }
}