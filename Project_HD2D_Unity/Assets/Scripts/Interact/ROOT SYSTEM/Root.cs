using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;
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
    [SerializeField] private int maxEnergy = 0;
    
    public int CurrentEnergy 
    { 
        get => currentEnergy;
        private set => SetEnergy(value); 
    }
    
    [Header("Root Visuals")]
    [SerializeField] private SplineContainer splineRoot;
    [SerializeField] private ArrayCurveSplineMesh splineScript;
    
    private readonly List<Renderer> childRenderers = new List<Renderer>();
    
    private MaterialPropertyBlock propBlockRoot;
    private static readonly int EnergyPropertyID = Shader.PropertyToID("_LineCount");
    private readonly Dictionary<object, Renderer> branchMap = new Dictionary<object, Renderer>();
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitFlaws();
        InitVatManagers();
        InitPropBlocks();
    }
    

    #endregion
    
    #region Init

    private void InitPropBlocks()
    {
        propBlockRoot = new MaterialPropertyBlock();
        RefreshChildRenderers();
        SetEnergy(CurrentEnergy);
    }

    

    private void RefreshChildRenderers()
    {
        childRenderers.Clear();
        branchMap.Clear();
        if (splineScript == null) return;

        Renderer[] rs = splineScript.GetComponentsInChildren<Renderer>();
        if (rs == null || rs.Length == 0) return;

        int index = 0;
        
        foreach (var flaw in flaws)
        {
            if (index < rs.Length) branchMap[flaw] = rs[index++];
        }
        
        foreach (var vat in vatManagers)
        {
            if (index < rs.Length) branchMap[vat] = rs[index++];
        }
    
        childRenderers.AddRange(rs);
    }

    public void UpdateVisualEnergy()
    {
        if (branchMap.Count == 0) RefreshChildRenderers();
        if (propBlockRoot == null) propBlockRoot = new MaterialPropertyBlock();

        foreach (var pair in branchMap)
        {
            Renderer r = pair.Value;
            if (r == null) continue;

            float energyToShow = (float)currentEnergy;

            if (pair.Key is IRootLink link && IsBranchBlocked(link))
            {
                energyToShow = 0f;
            }

            r.GetPropertyBlock(propBlockRoot);
            propBlockRoot.SetFloat(EnergyPropertyID, energyToShow);
            r.SetPropertyBlock(propBlockRoot);
        }
    }

    private bool IsBranchBlocked(IRootLink target)
    {
        if (target is Flaw flaw) return flaw.IsBlocked();
        if (target is VATManager vat) return vat.IsBlocked();
        return false;
    }
    
    private void InitFlaws()
    {
        foreach (Flaw flaw in flaws) flaw.SetRoot(this);
    }

    private void InitVatManagers()
    {
        foreach (VATManager vatManager in vatManagers) vatManager.SetRoot(this);
    }

    #endregion
    
    #region Link Flaws

    private void SetEnergy(int energy)
    {
        currentEnergy = Mathf.Clamp(energy, 0, maxEnergy);
        UpdateVisualEnergy();
    }


    public void AddEnergy()
    {
        CurrentEnergy++; 
        
        foreach (var vatManager in vatManagers)
        {
            if (vatManager.IsAtMaximumEnergy()) continue;
        }
    }

    public void RemoveEnergy()
    {
        CurrentEnergy--;
        
        foreach (var vatManager in vatManagers)
        {
            if (!vatManager.IsContainingEnergy()) continue;
        }
    }

    #endregion
    
    #region Helper 

    public bool IsContainingEnergy() => currentEnergy > 0;
    public bool IsAtMaximumEnergy()  => currentEnergy >= maxEnergy;

    #endregion
    
    #region Debug Gizmos
    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        
        GUIStyle debugStyle = new GUIStyle
        {
            normal = { textColor = Color.white },
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };

        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
        Handles.Label(transform.position + Vector3.up * 0.8f, "ENERGY: " + currentEnergy, debugStyle);

        float lineWidth = 4f; 
        
        if (flaws != null)
        {
            foreach (Flaw flaw in flaws)
            {
                if (flaw == null) continue;

                Color flawColor = flaw.IsBlocked() ? new Color(1f, 0.5f, 0f) : Color.blue;
                DrawThickLine(transform.position, flaw.transform.position, flawColor, lineWidth);
            
                Gizmos.color = flawColor;
                Gizmos.DrawWireSphere(flaw.transform.position, 0.4f);
            }
        }

        if (vatManagers != null)
        {
            foreach (VATManager vatManager in vatManagers)
            {
                if (vatManager == null) continue;

                Color vatColor = vatManager.IsBlocked() ? Color.red : Color.green;
                DrawThickLine(transform.position, vatManager.transform.position, vatColor, lineWidth);

                Gizmos.color = vatColor;
                Gizmos.DrawWireCube(vatManager.transform.position, Vector3.one * 0.6f);
            }
        }
#endif
    }

#if UNITY_EDITOR

    private void DrawThickLine(Vector3 start, Vector3 end, Color color, float width)
    {
        Handles.color = color;
        Handles.DrawBezier(start, end, start, end, color, null, width);
    }
#endif
    #endregion

    #region Save

    public void LoadData(GameData data)
    {
        RootSaveData myData = data.rootDataList.Find(x => x.id == entityID.ID);
        
        if (myData != null)
        {
            CurrentEnergy = myData.energy;
        }
    }

    public void SaveData(ref GameData data)
    {
        int index = data.rootDataList.FindIndex(x => x.id == entityID.ID);

        if (index != -1)
        {
            data.rootDataList[index].energy = CurrentEnergy;
        }
        else
        {
            data.rootDataList.Add(new RootSaveData { 
                id = entityID.ID, 
                energy = CurrentEnergy 
            });
        }
    }

    #endregion

    #region Bake Root Visuals

    [ContextMenu("Bake Visuals")]
    public void BakeVisuals()
    {
        if (splineRoot == null) return;

        ClearAllKnots();
    
        Vector3 localOrigin = splineRoot.transform.InverseTransformPoint(transform.position);

        if (flaws != null)
        {
            foreach (Flaw flaw in flaws)
            {
                if (flaw == null) continue;
                CreateSplineConnection(localOrigin, flaw.GetPositionRootVisuals());
            }
        }

        if (vatManagers != null)
        {
            foreach (VATManager vatManager in vatManagers)
            {
                if (vatManager == null) continue;
                CreateSplineConnection(localOrigin, vatManager.transform.position);
            }
        }
        
        splineScript.Rebuild();
        RefreshChildRenderers();
        UpdateVisualEnergy(); 
    }

    private void CreateSplineConnection(Vector3 localStart, Vector3 worldEnd)
    {
        Vector3 localEnd = splineRoot.transform.InverseTransformPoint(worldEnd);

        Spline newSpline = new Spline
        {
            new BezierKnot(localStart),
            new BezierKnot(localEnd)
        };

        splineRoot.AddSpline(newSpline);
    }
    
    [ContextMenu("Clear All Knots")]
    public void ClearAllKnots()
    {
        while (splineRoot.Splines.Count != 0)
        {
            splineRoot.RemoveSplineAt(0);
        }
        
        splineScript.Rebuild();
    }

    #endregion

    
}
