using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Splines;

public class Root : MonoBehaviour, IDataPersistence
{
    #region Variables

    [Header("Link VAT Managers")]
    public List<VATManager> vatManagers;

    [Header("Link Flaws")]
    public List<Flaw> flaws;

    [Header("Identification")]
    [SerializeField] private EntityID entityID;

    [Header("Current State")]
    [SerializeField] private int currentEnergy = 0;
    [SerializeField] private int maxEnergy     = 0;

    public int CurrentEnergy
    {
        get => currentEnergy;
        private set => SetEnergy(value);
    }

    [Header("Root Visuals")]
    [SerializeField] private SplineContainer      splineRoot;
    [SerializeField] private ArrayCurveSplineMesh splineScript;

    private readonly List<Renderer>               childRenderers = new List<Renderer>();
    private readonly Dictionary<object, Renderer> branchMap      = new Dictionary<object, Renderer>();

    private MaterialPropertyBlock propBlockRoot;
    private static readonly int   EnergyPropertyID = Shader.PropertyToID("_LineOffCount");

    [Header("Scan Timer")]
    [SerializeField] private float timerScanMax = 2f;
    private float timerScan = 0f;

    public bool Locked { get; set; }

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        InitFlaws();
        InitVatManagers();
        InitPropBlocks();
    }

    private void Update() => HandleVATShaders();

    #endregion

    #region Init

    private void InitFlaws()
    {
        foreach (var flaw in flaws) flaw.SetRoot(this);
    }

    private void InitVatManagers()
    {
        foreach (var vat in vatManagers) vat.SetRoot(this);
    }

    private void InitPropBlocks()
    {
        propBlockRoot = new MaterialPropertyBlock();
        RefreshChildRenderers();
        SetEnergy(currentEnergy);
    }

    private void RefreshChildRenderers()
    {
        childRenderers.Clear();
        branchMap.Clear();

        if (splineScript == null) return;

        Renderer[] rs = splineScript.GetComponentsInChildren<Renderer>();
        if (rs == null || rs.Length == 0) return;

        int index = 0;
        foreach (var flaw in flaws)      { if (index < rs.Length) branchMap[flaw] = rs[index++]; }
        foreach (var vat in vatManagers) { if (index < rs.Length) branchMap[vat]  = rs[index++]; }

        childRenderers.AddRange(rs);
    }

    #endregion

    #region Energy

    private void SetEnergy(int energy)
    {
        currentEnergy = Mathf.Clamp(energy, 0, maxEnergy);
        UpdateVisualEnergy();
    }

    public void AddEnergy()    => CurrentEnergy++;
    public void RemoveEnergy() => CurrentEnergy--;

    public bool IsContainingEnergy() => currentEnergy > 0;
    public bool IsAtMaximumEnergy()  => currentEnergy >= maxEnergy;

    #endregion

    #region Visuals

    public void UpdateVisualEnergy()
    {
        if (branchMap.Count == 0) RefreshChildRenderers();
        propBlockRoot ??= new MaterialPropertyBlock();

        foreach (var pair in branchMap)
        {
            Renderer r = pair.Value;
            if (r == null) continue;

            float energyToShow = pair.Key is IRootLink link && IsBranchBlocked(link)
                ? 0f
                : (float)currentEnergy;

            r.GetPropertyBlock(propBlockRoot);
            propBlockRoot.SetFloat(EnergyPropertyID, energyToShow);
            r.SetPropertyBlock(propBlockRoot);
        }
    }

    private bool IsBranchBlocked(IRootLink target)
    {
        if (target is Flaw flaw)      return flaw.IsBlocked();
        if (target is VATManager vat) return vat.IsBlocked();
        return false;
    }

    #endregion

    #region Shader VAT

    private void HandleVATShaders()
    {
        if (!Locked) return;

        timerScan += Time.deltaTime;
        if (timerScan < timerScanMax) return;

        timerScan = 0f;
        foreach (var vat in vatManagers)
            vat.HandleScanShader(true);
    }

    public void ResetScanTimer()
    {
        timerScan = 0f;
        foreach (var vat in vatManagers)
            vat.HandleScanShader(false);
    }

    #endregion

    #region Bake Root Visuals

    [ContextMenu("Bake Visuals")]
    public void BakeVisuals()
    {
        if (splineRoot == null) return;

        ClearAllKnots();

        Vector3 localOrigin = splineRoot.transform.InverseTransformPoint(transform.position);

        foreach (var flaw in flaws)
        {
            if (flaw == null) continue;
            CreateSplineConnection(localOrigin, flaw.GetPositionRootVisuals());
        }

        foreach (var vat in vatManagers)
        {
            if (vat == null) continue;
            CreateSplineConnection(localOrigin, vat.transform.position);
        }

        splineScript.Rebuild();
        RefreshChildRenderers();
        UpdateVisualEnergy();
    }

    private void CreateSplineConnection(Vector3 localStart, Vector3 worldEnd)
    {
        Vector3 localEnd = splineRoot.transform.InverseTransformPoint(worldEnd);

        splineRoot.AddSpline(new Spline
        {
            new BezierKnot(localStart),
            new BezierKnot(localEnd)
        });
    }

    [ContextMenu("Clear All Knots")]
    public void ClearAllKnots()
    {
        while (splineRoot.Splines.Count != 0)
            splineRoot.RemoveSplineAt(0);

        splineScript.Rebuild();
    }

    #endregion

    #region Save

    public void LoadData(GameData data)
    {
        RootSaveData myData = data.rootDataList.Find(x => x.id == entityID.ID);
        if (myData != null) CurrentEnergy = myData.energy;
    }

    public void SaveData(ref GameData data)
    {
        int index = data.rootDataList.FindIndex(x => x.id == entityID.ID);

        if (index != -1)
            data.rootDataList[index].energy = CurrentEnergy;
        else
            data.rootDataList.Add(new RootSaveData { id = entityID.ID, energy = CurrentEnergy });
    }

    #endregion

    #region Debug Gizmos
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        GUIStyle style = new GUIStyle
        {
            normal    = { textColor = Color.white },
            fontSize  = 16,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };

        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
        Handles.Label(transform.position + Vector3.up * 0.8f, "ENERGY: " + currentEnergy, style);

        const float lineWidth = 4f;

        foreach (var flaw in flaws)
        {
            if (flaw == null) continue;
            Color c = flaw.IsBlocked() ? new Color(1f, 0.5f, 0f) : Color.blue;
            DrawThickLine(transform.position, flaw.transform.position, c, lineWidth);
            Gizmos.color = c;
            Gizmos.DrawWireSphere(flaw.transform.position, 0.4f);
        }

        foreach (var vat in vatManagers)
        {
            if (vat == null) continue;
            Color c = vat.IsBlocked() ? Color.red : Color.green;
            DrawThickLine(transform.position, vat.transform.position, c, lineWidth);
            Gizmos.color = c;
            Gizmos.DrawWireCube(vat.transform.position, Vector3.one * 0.6f);
        }
    }

    private void DrawThickLine(Vector3 start, Vector3 end, Color color, float width)
    {
        Handles.color = color;
        Handles.DrawBezier(start, end, start, end, color, null, width);
    }
#endif
    #endregion
}