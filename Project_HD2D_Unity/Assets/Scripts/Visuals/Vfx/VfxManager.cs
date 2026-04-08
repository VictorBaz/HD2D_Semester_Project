using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VfxManager : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRendererDash;
    [SerializeField] private AttackComboFx[] attackComboFxs;

    private void Awake()
    {
        if (trailRendererDash != null) ToggleDashTrail(false);
    }

    public void ToggleDashTrail(bool isOn)
    {
        if (trailRendererDash != null) trailRendererDash.enabled = isOn;
    }

    public void PlayFxCombo(int index)
    {
        if (attackComboFxs == null || attackComboFxs.Length == 0) return;

        ClearAllComboFxs();

        foreach (var ps in attackComboFxs[index].particleSystems)
        {
            TriggerParticleSystem(ps);
        }
    }
    
    private void TriggerParticleSystem(ParticleSystem ps)
    {
        if (ps == null) return; 
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Play();
    }
    
    private void ClearAllComboFxs()
    {
        foreach (var attackFx in attackComboFxs)
        {
            foreach (var ps in attackFx.particleSystems.Where(ps => ps != null))
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }
}

[Serializable]
public class AttackComboFx
{
    public List<ParticleSystem> particleSystems;
}