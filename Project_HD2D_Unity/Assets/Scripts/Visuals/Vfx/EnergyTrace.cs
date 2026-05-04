using UnityEngine;

public class EnergyTrace : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public LineRenderer line;

    [Header("Line")]
    public int pointsCount = 32;

    [Header("Noise")]
    [Range(0.0f, 5.0f)]public float noiseStrength = 0.25f;
    [Range(0.0f, 1.0f)] public float noiseSpeed = 8f;

    [Header("Arc")]
    public float arcHeight = 1.5f;

    [Header("Particles")]
    public ParticleSystem particlePrefab;
    public int particleEmittersCount = 8;

    private ParticleSystem[] particleEmitters;

    void Start()
    {

        CreateParticleEmitters();
    }

    void Update()
    {
        if (!startPoint || !endPoint || !line) return;

        line.positionCount = pointsCount;

        for (int i = 0; i < pointsCount; i++)
        {
            float t = i / (float)(pointsCount - 1);

            Vector3 pos = Vector3.Lerp(startPoint.position, endPoint.position, t);

            // Arc vers le haut
            float arc = Mathf.Sin(t * Mathf.PI) * arcHeight;
            pos += Vector3.up * arc;

            // Noise
            Vector3 noise = new Vector3(
                Mathf.PerlinNoise(t * 4f, Time.time * noiseSpeed) - 0.5f,
                Mathf.PerlinNoise(t * 5f, Time.time * noiseSpeed + 10f) - 0.5f,
                Mathf.PerlinNoise(t * 6f, Time.time * noiseSpeed + 20f) - 0.5f
            );

            // Fade du noise aux extrémités
            float noiseMask = Mathf.Sin(t * Mathf.PI);
            pos += noise * noiseStrength * noiseMask;

            line.SetPosition(i, pos);
        }

        UpdateParticleEmitters();
    }

    void CreateParticleEmitters()
    {
        if (!particlePrefab || particleEmittersCount <= 0) return;

        particleEmitters = new ParticleSystem[particleEmittersCount];

        for (int i = 0; i < particleEmittersCount; i++)
        {
            ParticleSystem ps = Instantiate(particlePrefab, transform);
            particleEmitters[i] = ps;
        }
    }

void UpdateParticleEmitters()
{
    if (particleEmitters == null || line.positionCount < 2) return;

    for (int i = 0; i < particleEmitters.Length; i++)
    {
        if (!particleEmitters[i]) continue;

        float t = i / (float)(particleEmitters.Length - 1);

        float scaled = t * (line.positionCount - 1);
        int indexA = Mathf.FloorToInt(scaled);
        int indexB = Mathf.CeilToInt(scaled);

        Vector3 posA = line.GetPosition(indexA);
        Vector3 posB = line.GetPosition(indexB);

        float lerpT = scaled - indexA;

        Vector3 finalPos = Vector3.Lerp(posA, posB, lerpT);

        particleEmitters[i].transform.position = finalPos;
    }
}
}