using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxManagerPlayer : VfxManagerBase
{
    [Header("Dash Settings")]
    [SerializeField] private TrailRenderer trailRendererDash;

    [Header("Combo Settings")]
    [SerializeField] private AttackComboFx[] attackComboFxs;

    [Header("Shield Settings")]
    [SerializeField] private Renderer rendererShield;
    
    [Header("Energy Settings")]
    [SerializeField] private EnergyTrace energyTrace;
    public EnergyTrace EnergyTrace => energyTrace;

    [Header("Standalone Particles")]
    [SerializeField] private ParticleSystem psJump;
    [SerializeField] private ParticleSystem psParry;

    private MaterialPropertyBlock _propBlockShield;
    private Coroutine _shieldCoroutine;
    private static readonly int ProgressionId = Shader.PropertyToID("_Progression");
    
    private Vector3 _localOffsetParry;

    private void Awake()
    {
        _propBlockShield = new MaterialPropertyBlock();
        
        if (trailRendererDash != null) 
            ToggleDashTrail(false);
        
        if (rendererShield != null)
        {
            _propBlockShield.SetFloat(ProgressionId, 1f);
            rendererShield.SetPropertyBlock(_propBlockShield);
        }
        
        LinkVfx(false);

        if (psParry != null)
        {
            _localOffsetParry = psParry.transform.localPosition;
            psParry.transform.SetParent(null);
        }

        if (psJump != null)
        {
            psJump.transform.SetParent(null);
        }
    }

    #region Dash
    public void ToggleDashTrail(bool isOn)
    {
        if (trailRendererDash != null) 
            trailRendererDash.enabled = isOn;
    }
    #endregion

    #region Combos
    public void PlayFxCombo(int index)
    {
        if (attackComboFxs == null || index < 0 || index >= attackComboFxs.Length)
        {
            return;
        }

        ClearAllComboFxs();

        var combo = attackComboFxs[index];
        if (combo?.particleSystems == null) return;

        foreach (var ps in combo.particleSystems)
        {
            TriggerParticleSystem(ps);
        }
    }

    private void ClearAllComboFxs()
    {
        if (attackComboFxs == null) return;

        foreach (var attackFx in attackComboFxs)
        {
            if (attackFx?.particleSystems == null) continue;

            foreach (var ps in attackFx.particleSystems)
            {
                if (ps != null) 
                    StopParticleSystem(ps, true);
            }
        }
    }
    #endregion

    #region Shield
    public void CancelShield()
    {
        if (_shieldCoroutine != null)
            StopCoroutine(_shieldCoroutine);
        
        _propBlockShield.SetFloat(ProgressionId, 1f);
        rendererShield.SetPropertyBlock(_propBlockShield);
    }

    public void PlayParryVfx(float totalDuration, float pivotTime)
    {
        if (rendererShield == null) return;

        if (_shieldCoroutine != null) 
            StopCoroutine(_shieldCoroutine);

        _shieldCoroutine = StartCoroutine(ParryVfxRoutine(totalDuration, pivotTime));
    }

    private IEnumerator ParryVfxRoutine(float totalDuration, float pivotTime)
    {
        float elapsedTime = 0f;

        while (elapsedTime < totalDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentProgression;

            if (elapsedTime < pivotTime)
            {
                float ratio = elapsedTime / pivotTime;
                currentProgression = Mathf.Lerp(1f, 0f, ratio);
            }
            else
            {
                float segmentDuration = totalDuration - pivotTime;
                float ratio = (elapsedTime - pivotTime) / segmentDuration;
                currentProgression = Mathf.Lerp(0f, 1f, ratio);
            }

            _propBlockShield.SetFloat(ProgressionId, currentProgression);
            rendererShield.SetPropertyBlock(_propBlockShield);

            yield return null;
        }

        _propBlockShield.SetFloat(ProgressionId, 1f);
        rendererShield.SetPropertyBlock(_propBlockShield);
        _shieldCoroutine = null;
    }
    #endregion

    #region Link Vfx
    public void LinkVfx(bool isOn, Transform target = null)
    {
        if (!energyTrace) return;

        energyTrace.SetStaticEmittersActive(isOn);
        
        if (!target)
        {
            energyTrace.line.enabled = false;
            return;
        }
        
        energyTrace.line.enabled = isOn;
        energyTrace.startPoint = isOn ? energyTrace.transform : null;
        energyTrace.endPoint = isOn ? target : null;
    }
    #endregion
    
    public void UpdateLinkVisuals(bool parasite) => energyTrace?.SetParasiteMode(parasite);
    public void EffectAddEnergy() => energyTrace?.TriggerTraceFollow(0.5f, false);
    public void EffectRemoveEnergy() => energyTrace?.TriggerTraceFollow(0.5f, true);

    public void TriggerParticleJump(Vector3 position)
    {
        if (psJump == null) return;

        if (psJump.IsAlive())
            psJump.StopParticleSystem(true);
        
        psJump.transform.position = position;
        psJump.TriggerParticleSystem();
    }

    public void TriggerParryDone()
    {
        if (psParry == null) return;

        if (psParry.IsAlive())
            psParry.StopParticleSystem(true);

        psParry.transform.position = transform.TransformPoint(_localOffsetParry);
        float playerYAngle = transform.eulerAngles.y;
        float angleYOffset = 180f; 
        psParry.transform.rotation = Quaternion.Euler(0f, playerYAngle + angleYOffset, -90f);
        psParry.TriggerParticleSystem();
    }
}

[Serializable]
public class AttackComboFx
{
    public string name;
    public List<ParticleSystem> particleSystems;
}