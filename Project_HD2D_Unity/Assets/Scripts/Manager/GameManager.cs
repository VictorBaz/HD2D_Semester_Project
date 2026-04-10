using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameState initialState = GameState.Menu;
    
    private GameState currentState = GameState.Null;
    
    public GameState CurrentState => currentState;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ChangeState(initialState);
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        EventManager.TriggerGameStateChanged(newState);
    }

    

    public void TogglePause()
    {
        if (currentState == GameState.Game) ChangeState(GameState.Pause);
        else if (currentState == GameState.Pause) ChangeState(GameState.Game);
    }
}