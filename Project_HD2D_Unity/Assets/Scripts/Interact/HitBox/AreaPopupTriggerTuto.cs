using System.Collections.Generic;
using UnityEngine;

public class AreaPopupTriggerTuto : MonoBehaviour
{
    [Header("Popup Settings")]
    [SerializeField] private List<Sprite> popupSprites = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameConstants.PLAYER_TAG) && popupSprites.Count > 0)
        {
            UiEvents.TriggerShowSpritePopup(popupSprites);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GameConstants.PLAYER_TAG))
        {
            UiEvents.TriggerHideSpritePopup();
        }
    }
}
