using UnityEngine;

public class VfxManagerEnemy : VfxManagerBase
{
    [SerializeField] private ParticleSystem hitVfx;
    [SerializeField] private ParticleSystem attackVfx;
    [SerializeField] private ParticleSystem koVfx;

    public void PlayHitVfx()
    {
        TriggerParticleSystem(hitVfx);
    }

    public void TriggerAttackVfx()
    {
        TriggerParticleSystem(attackVfx);
    }

    public void SetKoVfx()
    {
        TriggerParticleSystem(koVfx);
    }
    
    public void StopKoVfx()
    {
        StopParticleSystem(koVfx, true);
    }
}