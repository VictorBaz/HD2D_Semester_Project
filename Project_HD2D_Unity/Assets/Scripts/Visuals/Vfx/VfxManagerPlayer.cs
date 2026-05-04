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
    [SerializeField] private EnergyTrace energyLink;

    private MaterialPropertyBlock _propBlockShield;
    private Coroutine _shieldCoroutine;
    private static readonly int ProgressionId = Shader.PropertyToID("_Progression");
    

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
            Debug.LogWarning($"[VfxManager] Index de combo invalide : {index}");
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
                    StopParticleSystem(ps,true);
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
        if (energyLink == null ) return;

        if (target == null)
        {
            energyLink.gameObject.SetActive(false);
            return;
        }
        
        energyLink.gameObject.SetActive(isOn);

        energyLink.startPoint = isOn ? energyLink.transform : null;
        energyLink.endPoint = isOn ? target : null;
    }

    #endregion

}

[Serializable]
public class AttackComboFx
{
    public string name;
    public List<ParticleSystem> particleSystems;
}