using System;
using UnityEngine;

public class EnergyDisplay : MonoBehaviour
{
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
                fullParticles[i].Play();
                emptyParticles[i].Stop();
            }
            else
            {
                fullParticles[i].Stop();
                emptyParticles[i].Play();
            }
        }
    }

    public void Hide()
    {
        for (int i = 0; i < fullParticles.Length; i++)
        {
            fullParticles[i].Stop();
            emptyParticles[i].Stop();
        }
    }
}
