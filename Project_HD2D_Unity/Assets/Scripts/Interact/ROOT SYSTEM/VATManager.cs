using System.Collections.Generic;
using UnityEngine;

public class VATManager : MonoBehaviour, IRootLink
{
    #region Variables
    [Header("VAT Settings")]
    [SerializeField] protected Renderer targetRenderer;
    [SerializeField] protected MeshFilter targetMeshFilter;
    [SerializeField] protected string shaderPropertyName = "_frame";
    [SerializeField] protected int maxFrames = 24;
    [SerializeField] protected List<float> animationSteps = new List<float> { 0f, 0.3f, 0.7f, 1f };
    [SerializeField] protected float transitionSpeed = 2f;
    [SerializeField] protected Animator animator;

    [Header("Blocking")]
    [SerializeField] private List<Parasite> blockers; 

    [Header("Root Visuals")]
    [SerializeField] private Transform rootVisual;
    
    protected float currentNormalizedValue = 0f;
    protected MaterialPropertyBlock propBlock;
    protected Root root;
    #endregion

    #region Unity Lifecycle
    protected virtual void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        SetupBounds(); 
        targetRenderer.staticShadowCaster = false; 
    }
    
    private void OnEnable()
    {
        foreach (var blocker in blockers)
        {
            blocker.OnDeath += UpdateRootVisuals;
        }
    }

    private void OnDisable()
    {
        foreach (var blocker in blockers)
        {
            blocker.OnDeath -= UpdateRootVisuals;
        }
    }

    protected virtual void Update() => UpdateVAT();
    #endregion

    #region VAT Methods
    protected void UpdateVAT()
    {
        int targetIndex = IsBlocked() ? 0 : Mathf.Clamp(CurrentEnergy, 0, MaxEnergyIndex);
        float targetValue = animationSteps[targetIndex];

        if (!Mathf.Approximately(currentNormalizedValue, targetValue))
        {
            currentNormalizedValue = Mathf.MoveTowards(
                currentNormalizedValue,
                targetValue,
                transitionSpeed * Time.deltaTime);
            
            OnValueUpdated(currentNormalizedValue);
        }
        
        ApplyVATToRenderer();
    }

    private void ApplyVATToRenderer()
    {
        float frameValue = currentNormalizedValue * Mathf.Max(0, maxFrames - 1);
        float clampedValue = Mathf.Clamp(currentNormalizedValue, 0, 0.99f);
        
        if(animator != null) animator.Play("Main Animation Vat", 0, clampedValue); 

        targetRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat(shaderPropertyName, frameValue);
        targetRenderer.SetPropertyBlock(propBlock);
    }
    #endregion

    #region Logic Checks
    public bool IsBlocked()
    {
        if (blockers == null) return false;
        blockers.RemoveAll(p => p == null);
        return blockers.Count > 0;
    }
    #endregion

    #region Helper Methods
    protected int CurrentEnergy => root != null ? root.CurrentEnergy : 0;
    protected int MaxEnergyIndex => animationSteps.Count - 1;

    public void SetRoot(Root root) => this.root = root;
    public bool IsContainingEnergy() => CurrentEnergy > 0 && !IsBlocked();
    public bool IsAtMaximumEnergy() => CurrentEnergy >= MaxEnergyIndex && !IsBlocked();

    private void SetupBounds()
    {
        if (targetRenderer == null || targetMeshFilter == null) return;
        Vector3 min = targetRenderer.sharedMaterial.GetVector("_minValues");
        Vector3 max = targetRenderer.sharedMaterial.GetVector("_maxValues");
        Vector3 center = (min + max) * 0.5f;
        Vector3 size = (max - min);
        targetMeshFilter.mesh.bounds = new Bounds(center, size);
    }

    protected virtual void OnValueUpdated(float newValue) { }
    
    public Vector3 GetPositionRootVisuals() => rootVisual.position;

    private void UpdateRootVisuals(Parasite parasite)
    {
        blockers.Remove(parasite);
        root.UpdateVisualEnergy();
    }
    #endregion
    
}