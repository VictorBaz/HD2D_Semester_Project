using UnityEngine;

public abstract class VfxManagerBase : MonoBehaviour
{
    [Header("Locomotion Settings")]
    [SerializeField] private ParticleSystem dustParticles;
    private bool isDustOn;
    
    
    protected void TriggerParticleSystem(ParticleSystem ps)
    {
        if (ps == null) return;
        
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Play();
    }

    protected void StopParticleSystem(ParticleSystem ps, bool clear = false)
    {
        if (ps == null) return;
        
        var behavior = clear ? ParticleSystemStopBehavior.StopEmittingAndClear : ParticleSystemStopBehavior.StopEmitting;
        ps.Stop(true, behavior);
    }
    
    public void PlayDust(bool newState)
    {
        if (dustParticles == null || newState == isDustOn) return;

        isDustOn = newState;

        if (isDustOn)
        {
            TriggerParticleSystem(dustParticles);
        }
        else
        {
            StopParticleSystem(dustParticles,true);
        }
    }
}