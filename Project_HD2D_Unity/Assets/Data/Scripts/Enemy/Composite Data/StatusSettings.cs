using UnityEngine;

[System.Serializable]
public class StatusSettings
{
    [Header("K-O & Stun")]
    public int MaxKo = 100;
    public float KoTimeMax = 5f;
    
    public float StunDuration = 0.2f;
    
    [Header("Exposed State")]
    public float ExposedTime = 1f;
    
}