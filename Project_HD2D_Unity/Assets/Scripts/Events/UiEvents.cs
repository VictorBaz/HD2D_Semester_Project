using System;
using UnityEngine;

public static class UiEvents
{
    public static event Action<bool> OnToggleInputPanel;
    public static void TriggerToggleInputPanel(bool show) => OnToggleInputPanel?.Invoke(show);

    public static event Action<int, int> OnEnergyChanged;
    public static void TriggerEnergyChanged(int current, int max) => OnEnergyChanged?.Invoke(current, max);

    public static event Action<int> OnSapChanged;
    public static event Action OnSapRemove;
    public static void TriggerSapChanged(int current) => OnSapChanged?.Invoke(current);
    public static void TriggerSapRemove() => OnSapRemove?.Invoke();

    public static event Action<bool> OnLockStateChanged;
    public static void TriggerLockStateChanged(bool isLocked) => OnLockStateChanged?.Invoke(isLocked);
    
    public static Action OnShowPopup;
    public static void TriggerShowPopup() 
        => OnShowPopup?.Invoke();
    
    public static Action<string> OnShowArea;
    public static void TriggerShowArea(string areaName) 
        => OnShowArea?.Invoke(areaName);
    
    public static event Action<System.Collections.Generic.List<Sprite>> OnShowSpritePopup;
    public static event Action OnHideSpritePopup;

    public static void TriggerShowSpritePopup(System.Collections.Generic.List<Sprite> sprites) => OnShowSpritePopup?.Invoke(sprites);
    public static void TriggerHideSpritePopup() => OnHideSpritePopup?.Invoke();
}