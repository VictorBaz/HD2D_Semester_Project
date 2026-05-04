using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PuzzleVisuals
{
    [SerializeField] private string shaderProperty = "_Corrupt";
    public List<Renderer> affectedRenderers = new();

    private MaterialPropertyBlock _propBlock;

    public void Initialize()
    {
        _propBlock = new MaterialPropertyBlock();
    }

    public void ApplyProgress(float progress)
    {
        if (_propBlock == null) Initialize();

        _propBlock.SetFloat(shaderProperty, progress);
        
        foreach (var rend in affectedRenderers)
        {
            if (rend != null)
            {
                rend.SetPropertyBlock(_propBlock);
            }
        }
    }

    public void ScanChildren(Transform root)
    {
        affectedRenderers.Clear();
        Renderer[] all = root.GetComponentsInChildren<Renderer>(true);
    
        foreach (var r in all)
        {
            bool hasProperty = false;
            foreach (var mat in r.sharedMaterials)
            {
                if (mat != null && mat.HasProperty(shaderProperty))
                {
                    hasProperty = true;
                    break; 
                }
            }

            if (hasProperty) affectedRenderers.Add(r);
        }
    }
}