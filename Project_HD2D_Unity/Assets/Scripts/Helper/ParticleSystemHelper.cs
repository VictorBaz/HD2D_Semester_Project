using UnityEngine;

public static class ParticleSystemHelper
{
    public static void TriggerParticleSystem(this ParticleSystem ps)
    {
        if (!ps) return;
        
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Play();
    }

    public static void StopParticleSystem(this ParticleSystem ps, bool clear = false)
    {
        if (!ps) return;
        
        var behavior = clear ? ParticleSystemStopBehavior.StopEmittingAndClear : ParticleSystemStopBehavior.StopEmitting;
        ps.Stop(true, behavior);
    }
    
    public static void SetSubEmittersProbability(this ParticleSystem ps, float probability)
    {
        var subEmitters = ps.subEmitters;
    
        for (int i = 0; i < subEmitters.subEmittersCount; i++)
        {
            subEmitters.SetSubEmitterEmitProbability(i, probability);
        }
    }
    
    public static void SetSubEmitterProbability(this ParticleSystem ps, int index, float probability)
    {
        var subEmitters = ps.subEmitters;

        if (index < 0 || index >= subEmitters.subEmittersCount) return;

        subEmitters.SetSubEmitterEmitProbability(index, probability);
    }
}