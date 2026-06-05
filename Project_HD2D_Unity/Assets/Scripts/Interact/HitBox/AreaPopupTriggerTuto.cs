using System;
using System.Collections.Generic;
using UnityEngine;

public class AreaPopupTriggerTuto : MonoBehaviour
{
    [Header("Popup Settings")]
    [SerializeField] private List<Sprite> popupSprites = new();
    [SerializeField] private bool blockPlayer = false;
    
    private bool shown = false;
    private PlayerManager player;

    private void OnTriggerEnter(Collider other)
    {
        if (shown || popupSprites.Count == 0) return;

        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;
        
        UiEvents.TriggerShowSpritePopup(popupSprites);

        if (blockPlayer)
        {
            player = other.GetComponentInParent<PlayerManager>();
            if (player)
            {
                player.ToggleMovement(true);
                player.GetInputManager().OnParry -= CancelTuto;
                player.GetInputManager().OnParry += CancelTuto;
            }
           
        }
        else
        {
            shown = true;
        }
    }

    private void CancelTuto()
    {
        UiEvents.TriggerHideSpritePopup();
        shown = true;

        if (!player) return;
        
        player.ToggleMovement(false);
            
        if (player.GetInputManager())
        {
            player.GetInputManager().OnParry -= CancelTuto;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (shown) return; 
        
        if (other.CompareTag(GameConstants.PLAYER_TAG))
        {
            UiEvents.TriggerHideSpritePopup();
            shown = true; 
        }
    }

    private void OnDisable()
    {
        if (player && player.GetInputManager())
        {
            player.GetInputManager().OnParry -= CancelTuto;
        }
    }
}