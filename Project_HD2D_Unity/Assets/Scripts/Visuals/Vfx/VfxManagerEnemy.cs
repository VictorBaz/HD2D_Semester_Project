using UnityEngine;

public class VfxManagerEnemy : VfxManagerBase
{
    [SerializeField] private ParticleSystem hitVfx;

    public void PlayHitVfx()
    {
        TriggerParticleSystem(hitVfx);
    }
}