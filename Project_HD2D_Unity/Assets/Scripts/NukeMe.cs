using UnityEngine;

public class NukeMe : MonoBehaviour
{
    public float GlobalCorruption = 0f;
    public float speed = 2f;

    private static readonly int CorruptionValueID = Shader.PropertyToID("_Corrupt");
    static readonly int CorruptID = Shader.PropertyToID("_Corrupt");
     public Material grassMaterial;

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            GlobalCorruption = Mathf.Lerp(GlobalCorruption, 1f, speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            GlobalCorruption = Mathf.Lerp(GlobalCorruption, 0f, speed * Time.deltaTime);
        }

        Shader.SetGlobalFloat(CorruptionValueID, GlobalCorruption);
         grassMaterial.SetFloat(CorruptID, GlobalCorruption);
    }
}