using UnityEngine;

public class VfxManagerEnemy : VfxManagerBase
{
    [SerializeField] private ParticleSystem hitVfx;
    [SerializeField] private ParticleSystem attackVfx;

    public void PlayHitVfx()
    {
        TriggerParticleSystem(hitVfx);
    }

    public void TriggerAttackVfx()
    {
        TriggerParticleSystem(attackVfx);
    }
}