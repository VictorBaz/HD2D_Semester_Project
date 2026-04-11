using System;

public static class UiEvents
{
    public static event Action<bool> OnToggleInputPanel;
    public static void TriggerToggleInputPanel(bool show) => OnToggleInputPanel?.Invoke(show);

    public static event Action<int, int> OnEnergyChanged;
    public static void TriggerEnergyChanged(int current, int max) => OnEnergyChanged?.Invoke(current, max);

    public static event Action<int, int> OnSapChanged;
    public static void TriggerSapChanged(int current, int max) => OnSapChanged?.Invoke(current, max);

    public static event Action<bool> OnLockStateChanged;
    public static void TriggerLockStateChanged(bool isLocked) => OnLockStateChanged?.Invoke(isLocked);
}