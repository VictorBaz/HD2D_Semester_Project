using System;
using System.Collections;
using UnityEngine;

public class EnergyTrace : MonoBehaviour
{
    [Header("References")]
    public Transform startPoint;
    public Transform endPoint {get; set;}
    public LineRenderer line;

    [Header("Line Settings")]
    public int pointsCount = 32;

    [Header("Noise Settings")]
    [Range(0f, 5f)] public float noiseStrength = 0.25f;
    [Range(0f, 1f)] public float noiseSpeed = 8f;

    [Header("Arc Settings")]
    public float arcHeight = 1.5f;

    [Header("Trace Particle (Pooled)")]
    [SerializeField] private ParticleSystem energyParticlePrefab;
    private const string POOL_KEY = "EnergyParticles";

    [Header("Static Emitters (Visual Decoration)")]
    public ParticleSystem particlePrefab;
    public int staticEmittersCount = 8;
    private ParticleSystem[] staticEmitters;
    private Renderer[]       staticEmitterRenderers;
    [SerializeField] private EnergyDataVisuals visuals;

    #region Unity Lifecycle

    void Start()
    {
        if (energyParticlePrefab != null)
            ObjectPooler.SetupPool(energyParticlePrefab, 15, POOL_KEY);

        CreateStaticEmitters();
        SetParasiteMode(false);
    }

    void Update()
    {
        if (!startPoint || !endPoint || !line) return;

        UpdateLine();
        UpdateStaticEmitters();
    }

    #endregion

    #region Line Drawing

    private void UpdateLine()
    {
        line.positionCount = pointsCount;

        for (int i = 0; i < pointsCount; i++)
        {
            float t = i / (float)(pointsCount - 1);
            Vector3 pos = SampleLinePosition(t);
            line.SetPosition(i, pos);
        }
    }

    private Vector3 SampleLinePosition(float t)
    {
        Vector3 pos = Vector3.Lerp(startPoint.position, endPoint.position, t);

        float arc = Mathf.Sin(t * Mathf.PI) * arcHeight;
        pos += Vector3.up * arc;

        float noiseMask = Mathf.Sin(t * Mathf.PI);
        pos += SampleNoise(t) * (noiseStrength * noiseMask);

        return pos;
    }

    private Vector3 SampleNoise(float t)
    {
        return new Vector3(
            Mathf.PerlinNoise(t * 4f, Time.time * noiseSpeed) - 0.5f,
            Mathf.PerlinNoise(t * 5f, Time.time * noiseSpeed + 10f) - 0.5f,
            Mathf.PerlinNoise(t * 6f, Time.time * noiseSpeed + 20f) - 0.5f
        );
    }

    #endregion

    #region Static Emitters

    private void CreateStaticEmitters()
    {
        if (!particlePrefab || staticEmittersCount <= 0) return;

        staticEmitters         = new ParticleSystem[staticEmittersCount];
        staticEmitterRenderers = new Renderer[staticEmittersCount];

        for (int i = 0; i < staticEmittersCount; i++)
        {
            staticEmitters[i]         = Instantiate(particlePrefab, transform);
            staticEmitterRenderers[i] = staticEmitters[i].GetComponent<Renderer>();
        }
        
        SetStaticEmittersActive(false);
    }

    private void UpdateStaticEmitters()
    {
        if (staticEmitters == null || line.positionCount < 2) return;

        for (int i = 0; i < staticEmitters.Length; i++)
        {
            if (!staticEmitters[i]) continue;

            float t = i / (float)(staticEmitters.Length - 1);
            staticEmitters[i].transform.position = GetPositionOnLine(t);
        }
    }

    #endregion

    #region Trace Follow

    public void TriggerTraceFollow(float duration, bool isReversed = false)
    {
        ParticleSystem ps = ObjectPooler.DequeueObject<ParticleSystem>(POOL_KEY);

        if (ps == null) return;

        ps.gameObject.SetActive(true);
        StartCoroutine(FollowTraceRoutine(ps, duration, isReversed));
    }

    private IEnumerator FollowTraceRoutine(ParticleSystem ps, float duration, bool isReversed)
    {
        ps.Play();
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (!ps || !ps.gameObject.activeInHierarchy)
            {
                ReturnToPool(ps);
                yield break;
            }

            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float t = isReversed ? 1f - progress : progress;

            if (line.positionCount >= 2)
            {
                (int indexA, int indexB, float segmentT) = GetLineIndices(t, isReversed);

                ps.transform.position = GetPositionOnLine(indexA, indexB, segmentT);
                ps.transform.forward  = GetDirectionOnLine(indexA, indexB, isReversed);
            }

            yield return null;
        }

        if (line.positionCount >= 2)
        {
            float finalT = isReversed ? 0f : 1f;
            ps.transform.position = GetPositionOnLine(finalT);
        }

        ReturnToPool(ps);
    }
    
    private void ReturnToPool(ParticleSystem ps)
    {
        if (ps == null) return;

        ps.Stop();
        ps.gameObject.SetActive(false); 
        ObjectPooler.EnqueueObject(ps, POOL_KEY);
    }

    #endregion

    #region Line Sampling Helpers

    private Vector3 GetPositionOnLine(float t)
    {
        float scaledIndex = t * (line.positionCount - 1);
        int indexA = Mathf.Clamp(Mathf.FloorToInt(scaledIndex), 0, line.positionCount - 1);
        int indexB = Mathf.Clamp(Mathf.CeilToInt(scaledIndex),  0, line.positionCount - 1);
        float segmentT = scaledIndex - indexA;

        return Vector3.Lerp(line.GetPosition(indexA), line.GetPosition(indexB), segmentT);
    }

    private Vector3 GetPositionOnLine(int indexA, int indexB, float segmentT)
    {
        return Vector3.Lerp(line.GetPosition(indexA), line.GetPosition(indexB), segmentT);
    }

    private (int indexA, int indexB, float segmentT) GetLineIndices(float t, bool isReversed)
    {
        float scaledIndex = t * (line.positionCount - 1);
        int indexA = Mathf.Clamp(Mathf.FloorToInt(scaledIndex), 0, line.positionCount - 1);

        int indexB = isReversed
            ? (indexA > 0                      ? indexA - 1 : indexA + 1)
            : (indexA < line.positionCount - 1 ? indexA + 1 : indexA - 1);

        float segmentT = Mathf.Abs(scaledIndex - indexA);
        return (indexA, indexB, segmentT);
    }

    private Vector3 GetDirectionOnLine(int indexA, int indexB, bool isReversed)
    {
        Vector3 direction = (line.GetPosition(indexB) - line.GetPosition(indexA)).normalized;
        return isReversed ? -direction : direction;
    }

    #endregion
    
    public void SetStaticEmittersActive(bool active)
    {
        if (staticEmitters == null) return;

        foreach (ParticleSystem emitter in staticEmitters)
        {
            if (!emitter) continue;

            if (active) emitter.Play();
            else        emitter.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
    
    public void SetParasiteMode(bool parasite)
    {
        line.sharedMaterial = visuals.GetLineMaterial(parasite);

        if (staticEmitterRenderers == null) return;
        Material mat = visuals.GetEmitterMaterial(parasite);

        foreach (Renderer r in staticEmitterRenderers)
        {
            if (r) r.sharedMaterial = mat;
        }
    }
}

[Serializable]
struct EnergyDataVisuals
{
    [Header("Line Renderer")]
    public Material lineMaterial;
    public Material lineParasiteMaterial;

    [Header("Static Emitters")]
    public Material emitterMaterial;
    public Material emitterParasiteMaterial;

    public Material GetLineMaterial(bool parasite)       => parasite ? lineParasiteMaterial    : lineMaterial;
    public Material GetEmitterMaterial(bool parasite)    => parasite ? emitterParasiteMaterial : emitterMaterial;
}