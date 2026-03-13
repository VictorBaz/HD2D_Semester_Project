// Exemple C# MINIMAL pour alimenter _Positions[] (obligatoire)
// Mat.SetVectorArray("_Positions", arr); Mat.SetFloat("_PositionCount", count);

using UnityEngine;

public class GrassInteractorsToShader : MonoBehaviour
{
    public Material grassMat;
    public Transform[] interactors; // joueur + autres
    public float radius = 1f;
    public float maxWidth = 0.1f;
    public bool debug = false;

    static readonly int PositionsID = Shader.PropertyToID("_Positions");
    static readonly int CountID     = Shader.PropertyToID("_PositionCount");
    static readonly int RadiusID    = Shader.PropertyToID("_Radius");
    static readonly int MaxWidthID  = Shader.PropertyToID("_MaxWidth");
    static readonly int DebugID     = Shader.PropertyToID("_DebugInteractor");

    Vector4[] buffer = new Vector4[100];

    void LateUpdate()
    {
        if (!grassMat) return;

        int count = Mathf.Min(interactors != null ? interactors.Length : 0, 100);

        for (int i = 0; i < count; i++)
        {
            Vector3 p = interactors[i] ? interactors[i].position : Vector3.zero;
            buffer[i] = new Vector4(p.x, p.y, p.z, 1);
        }

        grassMat.SetVectorArray(PositionsID, buffer);
        grassMat.SetFloat(CountID, count);
        grassMat.SetFloat(RadiusID, radius);
        grassMat.SetFloat(MaxWidthID, maxWidth);
        grassMat.SetFloat(DebugID, debug ? 1f : 0f);
    }
}