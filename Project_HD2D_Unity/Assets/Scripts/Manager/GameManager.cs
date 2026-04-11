using System;
using System.Collections;
using Enum;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameState initialState = GameState.Menu;

    private GameState currentState = GameState.Null;

    public GameState CurrentState => currentState;

    [SerializeField] private string GameSceneName;
    [SerializeField] private string MenuSceneName;

    private bool isLoading = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChangeState(initialState);
    }

    private void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        NewStateBehaviorTime(currentState);

        EventManager.TriggerGameStateChanged(newState);
    }

    private void NewStateBehaviorTime(GameState newState)
    {
        switch (newState)
        {
            case GameState.Menu:
            case GameState.Game:
            case GameState.Null:
                Time.timeScale = 1f;
                break;
            case GameState.Pause:
                Time.timeScale = 0f;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    public void TogglePause()
    {
        if (currentState == GameState.Game) ChangeState(GameState.Pause);
        else if (currentState == GameState.Pause) ChangeState(GameState.Game);
    }

    #region Loading Scene

    private void LoadScene(string sceneName)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadAsync(sceneName));
        }
    }

    private IEnumerator LoadAsync(string sceneName)
    {
        isLoading = true;

        EventManager.TriggerLoadingStarted();

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        isLoading = false;

        EventManager.TriggerLoadingFinished();
    }

    public void LoadGame() => LoadScene(GameSceneName);
    public void LoadMenu() => LoadScene(MenuSceneName);

    #endregion

    #region Helper

    public void ExecuteButtonAction(ButtonAction action)
    {
        switch (action)
        {
            case ButtonAction.Game:
                ChangeState(GameState.Game);
                LoadGame();
                break;
            case ButtonAction.ReturnToGame:
                ChangeState(GameState.Game);
                break;

            case ButtonAction.Pause:
                ChangeState(GameState.Pause);
                break;

            case ButtonAction.Menu:
                ChangeState(GameState.Menu);
                LoadMenu();
                break;

            case ButtonAction.Quit:
                Application.Quit();
                break;

            case ButtonAction.Settings:
                // EventManager.TriggerOpenSettings();
                break;

            case ButtonAction.Credits:
                break;
        }
    }

    #endregion
}