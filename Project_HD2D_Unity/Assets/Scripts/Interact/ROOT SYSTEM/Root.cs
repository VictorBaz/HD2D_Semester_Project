using System;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    #region Variables

    [Header("Link VAT Managers")] 
    public List<VATManager> vatManagers;
    
    [Header("Link Flaws")]
    public List<Flaw> flaws;
    
    [Header("Current State")]
    [SerializeField] private int currentEnergy = 0;
    [SerializeField] private int maxEnergy = 0;

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
            
            vatManager.AddEnergy();    
        }
    }

    public void RemoveEnergy()
    {
        SetEnergy(currentEnergy - 1);
        
        foreach (var vatManager in vatManagers)
        {
            if (!vatManager.IsContainingEnergy()) continue;
            
            vatManager.RemoveEnergy();    
        }
    }

    #endregion
    
    #region Helper 

    public bool IsContainingEnergy() => currentEnergy > 0;
    public bool IsAtMaximumEnergy()  => currentEnergy >= maxEnergy;

    #endregion
    
}
