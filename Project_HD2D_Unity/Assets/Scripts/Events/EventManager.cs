using System;

public static class EventManager
{
    public static event Action<GameState> OnGameStateChanged;
    public static void TriggerGameStateChanged(GameState newState) => OnGameStateChanged?.Invoke(newState);

    public static event Action OnLoadingStarted;
    public static void TriggerLoadingStarted() => OnLoadingStarted?.Invoke();

    public static event Action OnLoadingFinished;
    public static void TriggerLoadingFinished() => OnLoadingFinished?.Invoke();
}