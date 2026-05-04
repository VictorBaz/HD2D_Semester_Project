using System;
using System.Collections;
using Enum;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton & Variables
    
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameState initialState = GameState.Menu;

    private GameState currentState = GameState.Null;
    private GameState previousState = GameState.Null; 

    public GameState CurrentState => currentState;
    public GameState PreviousState => previousState;

    [SerializeField] private string GameSceneName;
    [SerializeField] private string MenuSceneName;

    private bool isLoading = false;
    
    #endregion

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

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        previousState = currentState;
        
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
            case GameState.Credits:
            case GameState.Null:
                Time.timeScale = 1f;
                break;
            case GameState.Settings:
            case GameState.Pause:
                Time.timeScale = 0f;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    public void GoBack()
    {
        if (previousState != GameState.Null)
        {
            ChangeState(previousState);
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
            case ButtonAction.LoadGame:
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
                
                if (SceneManager.GetActiveScene().name != MenuSceneName)
                {
                    LoadMenu();
                }
                
                ChangeState(GameState.Menu);
                break;

            case ButtonAction.Credits:
                ChangeState(GameState.Credits);
                break;

            case ButtonAction.Quit:
                Application.Quit();
                break;
            case ButtonAction.Settings:
                ChangeState(GameState.Settings);
                break;
            case ButtonAction.SaveGame:
                DataPersistenceManager.Instance.SaveGame();
                break;
            case ButtonAction.NewGame:
                DataPersistenceManager.Instance.NewGame();
                ChangeState(GameState.Game);
                LoadGame();
                break;
        }
    }

    #endregion
}