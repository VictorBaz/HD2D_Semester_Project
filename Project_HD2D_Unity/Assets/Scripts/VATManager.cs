using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VATManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private string shaderPropertyName = "_frame";

    [Header("Animation Settings")]
    [SerializeField] private int maxFrames = 24;
    [Tooltip("Chaque palier correspond à 1 point d'énergie. Indice 0 = 0 énergie.")]
    [SerializeField] private List<float> animationSteps = new List<float> { 0f, 0.3f, 0.7f, 1f };
    [SerializeField] private float transitionSpeed = 2f;
    [SerializeField] public Animator animator;

    [Header("Current State")]
    [SerializeField] private int currentEnergy = 0;

    private float currentNormalizedValue = 0f;
    private MaterialPropertyBlock propBlock;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        propBlock = new MaterialPropertyBlock();

        if (animationSteps.Count > 0)
            currentNormalizedValue = animationSteps[0];
    }
    
    private void Update()
    {
        UpdateStep();
    }

    #endregion

    #region VAT

    public void SetEnergy(int energy)
    {
        currentEnergy = Mathf.Clamp(energy, 0, animationSteps.Count - 1);
    }

    public void AddEnergy() => SetEnergy(currentEnergy + 1);
    public void RemoveEnergy() => SetEnergy(currentEnergy - 1);
    

    private void UpdateStep()
    {
        int   targetIndex = Mathf.Clamp(currentEnergy, 0, animationSteps.Count - 1);
        float targetValue = animationSteps[targetIndex];

        if (!Mathf.Approximately(currentNormalizedValue, targetValue))
        {
            currentNormalizedValue = Mathf.MoveTowards(
                currentNormalizedValue,
                targetValue,
                transitionSpeed * Time.deltaTime);
        }
        
        float frameValue = currentNormalizedValue * Mathf.Max(0, maxFrames - 1);
        
        animator.Play("AnimClip", -1, currentNormalizedValue); 

        targetRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat(shaderPropertyName, frameValue);
        targetRenderer.SetPropertyBlock(propBlock);
    }


    #endregion

    #region Collider


    #endregion 

    public bool IsContainingEnergy() => currentEnergy > 0;
    public bool IsAtMaximumEnergy()  => currentEnergy >= animationSteps.Count - 1;
}