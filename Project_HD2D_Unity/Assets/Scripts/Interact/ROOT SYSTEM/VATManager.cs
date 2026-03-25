using System.Collections.Generic;
using UnityEngine;

public class VATManager : MonoBehaviour, IRootLink
{
    [Header("VAT Settings")]
    [SerializeField] protected Renderer targetRenderer;
    [SerializeField] protected string shaderPropertyName = "_frame";
    [SerializeField] protected int maxFrames = 24;
    [SerializeField] protected List<float> animationSteps = new List<float> { 0f, 0.3f, 0.7f, 1f };
    [SerializeField] protected float transitionSpeed = 2f;
    [SerializeField] protected Animator animator;

    protected float currentNormalizedValue = 0f;
    protected MaterialPropertyBlock propBlock;
    protected Root root;

    protected int CurrentEnergy => root != null ? root.currentEnergy : 0;
    protected int MaxEnergyIndex => animationSteps.Count - 1;

    protected virtual void Awake() => propBlock = new MaterialPropertyBlock();

    protected virtual void Update() => UpdateVAT();

    protected void UpdateVAT()
    {
        int targetIndex = Mathf.Clamp(CurrentEnergy, 0, MaxEnergyIndex);
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

    protected virtual void OnValueUpdated(float newValue) { }

    public void SetRoot(Root root) => this.root = root;
    public bool IsContainingEnergy() => CurrentEnergy > 0;
    public bool IsAtMaximumEnergy() => CurrentEnergy >= MaxEnergyIndex;
}