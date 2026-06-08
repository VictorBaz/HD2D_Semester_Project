using System;
using UnityEngine;

public class EnergyDisplay : MonoBehaviour
{
    private static readonly int IsFull = Shader.PropertyToID("IsFull");
    
    [SerializeField] private ParticleSystem[] fullParticles;
    [SerializeField] private ParticleSystem[] emptyParticles;

    private void Start()
    {
        Hide();
    }

    public void Show(int energyLevel)
    {
        for (int i = 0; i < fullParticles.Length; i++)
        {
            if (i + 1 <= energyLevel)
            {
                fullParticles[i].gameObject.SetActive(true);
                emptyParticles[i].gameObject.SetActive(false);
            }
            else
            {
                fullParticles[i].gameObject.SetActive(false);
                emptyParticles[i].gameObject.SetActive(true);
            }
        }
    }

    public void Hide()
    {
        for (int i = 0; i < emptyParticles.Length; i++)
        {
            emptyParticles[i].gameObject.SetActive(false);
            fullParticles[i].gameObject.SetActive(false);
        }
    }
}
