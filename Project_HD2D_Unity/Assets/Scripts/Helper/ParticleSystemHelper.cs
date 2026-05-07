using UnityEngine;

public static class ParticleSystemHelper
{
    public static void TriggerParticleSystem(this ParticleSystem ps)
    {
        if (ps == null) return;
        
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Play();
    }

    public static void StopParticleSystem(this ParticleSystem ps, bool clear = false)
    {
        if (ps == null) return;
        
        var behavior = clear ? ParticleSystemStopBehavior.StopEmittingAndClear : ParticleSystemStopBehavior.StopEmitting;
        ps.Stop(true, behavior);
    }
}