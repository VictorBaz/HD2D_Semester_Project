using System;
using UnityEngine;

public class EnergyDisplay : MonoBehaviour
{
    private static readonly int IsFull = Shader.PropertyToID("IsFull");
    
    [SerializeField] private ParticleSystemRenderer[] particles;

    private void Start()
    {
        Hide();
    }

    public void Show(int energyLevel)
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].gameObject.SetActive(true);
            
            if (i + 1 <= energyLevel)
            {
                particles[i].material.SetFloat(IsFull, 1.0f);
            }
            else
            {
                particles[i].material.SetFloat(IsFull, 0.0f);
            }
        }
    }

    public void Hide()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].gameObject.SetActive(false);
        }
    }
}
