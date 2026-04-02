using UnityEngine;

[System.Serializable]
public class StatusSettings
{
    [Header("K-O & Stun")]
    public int MaxKo = 100;
    public float KoTime = 15f;
    public float StunDuration = 0.2f;
    
    [Header("Exposed State")]
    public float ExposedTime = 1f;
    
    [Header("Damage")]
    public int DamageToApply = 1;
}