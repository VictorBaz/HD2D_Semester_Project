using UnityEngine;
using System.Collections.Generic;

public class EnergyPlatform : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("Chaque palier correspond à 1 point d'énergie. Indice 0 = 0 énergie.")]
    [SerializeField] private List<float> animationSteps = new List<float> { 0f, 0.3f, 0.7f, 1f };
    
    [SerializeField] private float transitionSpeed = 2f;
    [SerializeField] private string shaderPropertyName = "test";

    [Header("Current State")]
    [SerializeField] private int currentEnergy = 0;
    private float currentNormalizedValue = 0f;
    private Renderer platformRenderer;
    private MaterialPropertyBlock propBlock;

    void Awake()
    {
        platformRenderer = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        
        if (animationSteps.Count > 0)
            currentNormalizedValue = animationSteps[0];
    }

    void Update()
    {
        
        int targetIndex = Mathf.Clamp(currentEnergy, 0, animationSteps.Count - 1);
        float targetValue = animationSteps[targetIndex];
        
        if (!Mathf.Approximately(currentNormalizedValue, targetValue))
        {
            currentNormalizedValue = Mathf.MoveTowards(
                currentNormalizedValue, 
                targetValue, 
                transitionSpeed * Time.deltaTime
            );
            
            platformRenderer.GetPropertyBlock(propBlock);
            propBlock.SetFloat(shaderPropertyName, currentNormalizedValue);
            platformRenderer.SetPropertyBlock(propBlock);
        }
    }
    
    public void ChangeEnergy(int amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0, animationSteps.Count - 1);
        Debug.Log($"Énergie plateforme : {currentEnergy} | Cible : {animationSteps[currentEnergy]}");
    }
}