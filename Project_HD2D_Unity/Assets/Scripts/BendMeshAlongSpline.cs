using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(SplineContainer))]
public class ArrayCurveSplineMesh : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] private Mesh sourceMesh;

    [Header("Array")]
    [SerializeField] private Axis forwardAxis = Axis.X;
    [SerializeField] private float spacing = 0f;
    [SerializeField] private bool fitToSplineLength = true;
    [SerializeField] private int manualCount = 1;

    [Header("Curve")]
    [SerializeField] private bool useSplineUp = false;

    [Header("Editor")]
    [SerializeField] private bool rebuildInEditor = true;
    [SerializeField] private bool autoUpdateEveryFrame = false;

    private MeshFilter meshFilter;
    [SerializeField] private SplineContainer splineContainer;
    private List<Mesh> meshesRootsVisual = new List<Mesh>();
    private Mesh generatedMesh;


    public enum Axis
    {
        X,
        Y,
        Z
    }

    private void OnEnable()
    {
        Cache();
        Rebuild();
    }

    private void OnValidate()
    {
        if (!rebuildInEditor)
            return;

        Cache();
        Rebuild();
    }

    private void Update()
    {
        if (!autoUpdateEveryFrame)
            return;

        Rebuild();
    }

    private void Cache()
    {
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();

        if (splineContainer == null)
            splineContainer = GetComponent<SplineContainer>();
        meshesRootsVisual.Clear();
    }

    [ContextMenu("Rebuild")]
   [ContextMenu("Rebuild")]
public void Rebuild()
{
    Cache();
    ClearAllGeneratedMeshes();

    if (sourceMesh == null)
    {
        Debug.LogError("ArrayCurveSplineMesh : sourceMesh non assigné.", this);
        return;
    }

    if (splineContainer == null /*|| splineContainer.Spline == null*/)
    {
        Debug.LogError("ArrayCurveSplineMesh : SplineContainer ou Spline manquant.", this);
        return;
    }

    for (int i = 0; i < splineContainer.Splines.Count; i++)
    {
        Spline spline = splineContainer.Splines[i];
        float splineLength = spline.GetLength();

        if (splineLength <= 0.0001f)
            continue;

        Vector3[] srcVerts = sourceMesh.vertices;
        Vector3[] srcNormals = sourceMesh.normals;
        Vector2[] srcUVs = sourceMesh.uv;
        int[] srcTriangles = sourceMesh.triangles;

        Bounds bounds = sourceMesh.bounds;
        float minAlong = GetAxis(bounds.min, forwardAxis);
        float maxAlong = GetAxis(bounds.max, forwardAxis);
        float segmentLength = maxAlong - minAlong;

        if (segmentLength <= 0.0001f)
            continue;

        float step = segmentLength + spacing;
        int count = fitToSplineLength
            ? Mathf.Max(1, Mathf.CeilToInt((splineLength - spacing) / step))
            : Mathf.Max(1, manualCount);

        List<Vector3> combinedVerts = new List<Vector3>();
        List<Vector3> combinedNormals = new List<Vector3>();
        List<Vector2> combinedUVs = new List<Vector2>();
        List<int> combinedTriangles = new List<int>();

        for (int copyIndex = 0; copyIndex < count; copyIndex++)
        {
            float offset = copyIndex * step;
            int vertexStart = combinedVerts.Count;

            for (int j = 0; j < srcVerts.Length; j++)
            {
                Vector3 v = srcVerts[j];
                Vector3 n = (srcNormals != null && srcNormals.Length == srcVerts.Length) ? srcNormals[j] : Vector3.up;

                SetAxis(ref v, forwardAxis, GetAxis(v, forwardAxis) + offset);
                combinedVerts.Add(v);
                combinedNormals.Add(n);

                if (srcUVs != null && srcUVs.Length == srcVerts.Length)
                {
                    Vector2 uv = srcUVs[j];
                    float localAlong = GetAxis(srcVerts[j], forwardAxis) - minAlong;
                    float globalAlong = (copyIndex * segmentLength) + localAlong;
                    float totalLength = count * segmentLength;
                    uv.y = globalAlong / totalLength;
                    combinedUVs.Add(uv);
                }
                else
                {
                    float localAlong = GetAxis(srcVerts[j], forwardAxis) - minAlong;
                    float globalAlong = (copyIndex * segmentLength) + localAlong;
                    float totalLength = count * segmentLength;
                    combinedUVs.Add(new Vector2(0f, globalAlong / totalLength));
                }
            }

            for (int k = 0; k < srcTriangles.Length; k++)
                combinedTriangles.Add(vertexStart + srcTriangles[k]);
        }

        float combinedMin = minAlong;
        float combinedMax = minAlong + (count * segmentLength) + ((count - 1) * spacing);

        Vector3[] deformedVerts = new Vector3[combinedVerts.Count];
        Vector3[] deformedNormals = new Vector3[combinedNormals.Count];

        Transform splineTransform = splineContainer.transform;
        Matrix4x4 worldToLocal = transform.worldToLocalMatrix;

        for (int l = 0; l < combinedVerts.Count; l++)
        {
            Vector3 v = combinedVerts[l];

            float along = GetAxis(v, forwardAxis);
            float alpha = Mathf.InverseLerp(combinedMin, combinedMax, along);
            float distanceOnSpline = alpha * splineLength;

            float t = SplineUtility.ConvertIndexUnit(
                spline,
                distanceOnSpline,
                PathIndexUnit.Distance,
                PathIndexUnit.Normalized
            );

            Vector3 splinePos = (Vector3)spline.EvaluatePosition(t);
            Vector3 tangent = ((Vector3)spline.EvaluateTangent(t)).normalized;
            if (tangent.sqrMagnitude < 0.000001f) tangent = Vector3.forward;

            Vector3 up = useSplineUp
                ? ((Vector3)spline.EvaluateUpVector(t)).normalized
                : Vector3.up;
            if (up.sqrMagnitude < 0.000001f) up = Vector3.up;

            Vector3 right = Vector3.Cross(tangent, up).normalized;
            if (right.sqrMagnitude < 0.000001f)
            {
                right = Vector3.right;
                up = Vector3.Cross(tangent, right).normalized;
            }
            else
            {
                up = Vector3.Cross(tangent, right).normalized;
            }

            Vector2 cross = GetCrossCoordinates(v, forwardAxis);
            Vector3 localOffset = right * cross.x + up * cross.y;

            Vector3 deformedInSplineLocal = splinePos + localOffset;
            Vector3 deformedWorld = splineTransform.TransformPoint(deformedInSplineLocal);
            deformedVerts[l] = worldToLocal.MultiplyPoint3x4(deformedWorld);

            Vector3 n = combinedNormals[l];
            float nAlong = GetAxis(n, forwardAxis);
            Vector2 nCross = GetCrossCoordinates(n, forwardAxis);

            Vector3 deformedNormalInSplineLocal =
                tangent * nAlong +
                right * nCross.x +
                up * nCross.y;

            Vector3 deformedNormalWorld = splineTransform.TransformDirection(deformedNormalInSplineLocal);
            deformedNormals[l] = worldToLocal.MultiplyVector(deformedNormalWorld).normalized;
        }

        Mesh splineMesh = new Mesh();
        splineMesh.name = sourceMesh.name + "_ArrayCurve_" + i;
        splineMesh.indexFormat = deformedVerts.Length > 65535
            ? UnityEngine.Rendering.IndexFormat.UInt32
            : UnityEngine.Rendering.IndexFormat.UInt16;

        splineMesh.vertices = deformedVerts;
        splineMesh.triangles = combinedTriangles.ToArray();
        splineMesh.uv = combinedUVs.ToArray();
        splineMesh.normals = deformedNormals;
        splineMesh.RecalculateBounds();
        splineMesh.RecalculateTangents();

        meshesRootsVisual.Add(splineMesh);
    }

    if (meshesRootsVisual.Count == 0)
    {
        meshFilter.sharedMesh = null;
        return;
    }

    CombineInstance[] combine = new CombineInstance[meshesRootsVisual.Count];

    for (int i = 0; i < meshesRootsVisual.Count; i++)
    {
        combine[i].mesh = meshesRootsVisual[i];
        combine[i].transform = Matrix4x4.identity;
    }

    Mesh combinedMesh = new Mesh();
    combinedMesh.name = sourceMesh.name + "_Combined";
    combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
    combinedMesh.CombineMeshes(combine, true, false);

    meshFilter.sharedMesh = combinedMesh;
}
private void ClearAllGeneratedMeshes()
{
    foreach (var mesh in meshesRootsVisual)
    {
        if (mesh == null) continue;

        if (Application.isPlaying)
            Destroy(mesh);
        else
            DestroyImmediate(mesh);
    }

    meshesRootsVisual.Clear();
}

    private float GetAxis(Vector3 v, Axis axis)
    {
        switch (axis)
        {
            case Axis.X: return v.x;
            case Axis.Y: return v.y;
            case Axis.Z: return v.z;
            default: return v.x;
        }
    }

    private void SetAxis(ref Vector3 v, Axis axis, float value)
    {
        switch (axis)
        {
            case Axis.X: v.x = value; break;
            case Axis.Y: v.y = value; break;
            case Axis.Z: v.z = value; break;
        }
    }

    private Vector2 GetCrossCoordinates(Vector3 v, Axis axis)
    {
        switch (axis)
        {
            case Axis.X:
                return new Vector2(v.z, v.y);

            case Axis.Y:
                return new Vector2(v.x, v.z);

            case Axis.Z:
                return new Vector2(v.x, v.y);

            default:
                return new Vector2(v.z, v.y);
        }
    }
}