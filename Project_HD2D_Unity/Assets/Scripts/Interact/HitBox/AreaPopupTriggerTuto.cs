using System;
using System.Collections.Generic;
using UnityEngine;

public class AreaPopupTriggerTuto : MonoBehaviour
{
    [Header("Popup Settings")]
    [SerializeField] private List<Sprite> popupSprites = new();
    private PlayerManager player;
    private bool visited = false;

    private void OnTriggerEnter(Collider other)
    {
        if (popupSprites.Count == 0 || visited) return;

        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;
        
        UiEvents.TriggerShowSpritePopup(popupSprites);

        player = other.GetComponentInParent<PlayerManager>();
        
        if (player)
        {
            player.TogglePlayer(false);
            player.GetInputManager().OnAttackMelee -= CancelTuto;
            player.GetInputManager().OnAttackMelee += CancelTuto;
        }
        
        visited = true;
    }

    private void CancelTuto()
    {
        UiEvents.TriggerHideSpritePopup();

        if (!player) return;
        
        player.TogglePlayer(true);
            
        if (player.GetInputManager())
        {
            player.GetInputManager().OnAttackMelee -= CancelTuto;
        }
    }

    private void OnDisable()
    {
        if (player && player.GetInputManager())
        {
            player.GetInputManager().OnAttackMelee -= CancelTuto;
        }
    }
}