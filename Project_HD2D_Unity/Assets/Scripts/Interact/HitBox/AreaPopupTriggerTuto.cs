using System;
using System.Collections.Generic;
using UnityEngine;

public class AreaPopupTriggerTuto : MonoBehaviour
{
    [Header("Popup Settings")]
    [SerializeField] private List<Sprite> popupSprites = new();
    private PlayerManager player;
    private bool visited = false;
    [field: SerializeField, TextArea] private string description = "";

    private void OnTriggerEnter(Collider other)
    {
        if (popupSprites.Count == 0 || visited) return;

        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;

        player = other.GetComponentInParent<PlayerManager>();
        
        if (player)
        {
            UiEvents.TriggerShowSpritePopup(popupSprites,description);

            if (player.Context != null && player.Context.Rb != null)
            {
                player.Context.Rb.linearVelocity = Vector3.zero;
            }

            GameplayEvents.TriggerPlayerEnable(false);

            if (player.GetInputManager() != null)
            {
                player.GetInputManager().OnAttackMelee -= CancelTuto;
                player.GetInputManager().OnAttackMelee += CancelTuto;
            }
            
            visited = true;
        }
    }

    private void CancelTuto()
    {
        UiEvents.TriggerHideSpritePopup();

        if (player)
        {
            GameplayEvents.TriggerPlayerEnable(true);
            
            if (player.GetInputManager() != null)
            {
                player.GetInputManager().OnAttackMelee -= CancelTuto;
            }
        }
    }

    private void OnDisable()
    {
        if (player && player.GetInputManager() != null)
        {
            player.GetInputManager().OnAttackMelee -= CancelTuto;
        }
    }
}