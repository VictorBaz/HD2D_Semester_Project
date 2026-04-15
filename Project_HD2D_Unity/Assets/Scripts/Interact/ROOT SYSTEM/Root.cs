using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

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
    
    public int CurrentEnergy => currentEnergy;

    #endregion
    
    #region Unity Lifecycle

    private void Awake()
    {
        InitFlaws();
        InitVatManagers();
    }

    #endregion
    
    #region Init

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
    }

    public void AddEnergy()
    {
        SetEnergy(currentEnergy + 1);
        
        foreach (var vatManager in vatManagers)
        {
            if (vatManager.IsAtMaximumEnergy()) continue;
        }
    }

    public void RemoveEnergy()
    {
        SetEnergy(currentEnergy - 1);
        
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
            this.currentEnergy = myData.energy;
        }
    }

    public void SaveData(ref GameData data)
    {
        int index = data.rootDataList.FindIndex(x => x.id == entityID.ID);

        if (index != -1)
        {
            data.rootDataList[index].energy = this.currentEnergy;
        }
        else
        {
            data.rootDataList.Add(new RootSaveData { 
                id = entityID.ID, 
                energy = this.currentEnergy 
            });
        }
    }

    #endregion
}
