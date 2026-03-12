using UnityEngine;

public class VATManager : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;

    [Range(0f, 1f)]
    [SerializeField] private float normalizedAnimationRange = 0f;

    //hardcoder mais faut que je modifie pour que ca prenne direct les infos du json d'export vat
    [SerializeField] private int maxFrames = 24;

    private void Update()
    {
        float frameValue = normalizedAnimationRange * Mathf.Max(0, maxFrames - 1);
        targetRenderer.material.SetFloat("_frame", frameValue);
    }
}
