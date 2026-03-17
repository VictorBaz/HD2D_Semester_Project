using System;
using UnityEngine;

public class NukeMe : MonoBehaviour
{
    public float GlobalCorruption = 0f;
    public float duration = 1f;

    private float timer = 0f;

    private static readonly int CorruptionValueID = Shader.PropertyToID("_Corrupt");

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            GlobalCorruption = Mathf.Lerp(GlobalCorruption, 1f, 2f * Time.deltaTime);
            Debug.Log(GlobalCorruption);
            Shader.SetGlobalFloat(CorruptionValueID, GlobalCorruption);
        }

         if (Input.GetKey(KeyCode.Q))
        {
            GlobalCorruption = Mathf.Lerp(GlobalCorruption, 0f, 2f * Time.deltaTime);
            Debug.Log(GlobalCorruption);
            Shader.SetGlobalFloat(CorruptionValueID, GlobalCorruption);
        }
    }
}