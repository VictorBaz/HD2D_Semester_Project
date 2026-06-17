using System;
using UnityEngine;

public class ZoneAreName : MonoBehaviour
{
    [TextArea] [SerializeField] private string areaName;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameConstants.PLAYER_TAG))
        {
            UiEvents.TriggerShowArea(areaName);
        }
    }
}
