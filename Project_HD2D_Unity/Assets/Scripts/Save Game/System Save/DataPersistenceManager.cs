using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    #region Variables & Singleton

    [Header("Debug")]
    [SerializeField] private bool disableSave = false;
    [SerializeField] private bool initDataIfNull = false;

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private GameData gameData;

    private List<IDataPersistence> dataPersistencesObjects;
    private FileDataHandler dataHandler;

    public static DataPersistenceManager Instance { get; private set; }

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("There's more than one DataPersistenceManager in the scene! Destroying the newest one.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (disableSave)
            Debug.LogWarning("Data persistence is currently disabled.");

        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    #endregion

    #region Scene Load

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataPersistencesObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    #endregion

    #region Save / Load

    public void NewGame()
    {
        gameData = new GameData();
        SaveGame();
    }

    public void LoadGame()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (disableSave || sceneIndex == 0) return;

        gameData = dataHandler.Load();

        if (gameData == null && initDataIfNull)
        {
            Debug.Log("No data found — creating new game data.");
            NewGame();
        }

        if (gameData == null)
        {
            Debug.Log("No data found. A new game must be created first.");
            return;
        }

        foreach (IDataPersistence obj in dataPersistencesObjects)
            obj.LoadData(gameData);
    }

    public void SaveGame()
    {
        if (disableSave) return;

        if (gameData == null)
        {
            Debug.LogWarning("No data found. A new game must be created first.");
            return;
        }

        foreach (IDataPersistence obj in dataPersistencesObjects)
            obj.SaveData(ref gameData);

        gameData.LastUpdated = System.DateTime.Now.ToBinary();

        dataHandler.Save(gameData);
    }

    public bool HasGameData() => gameData != null;

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IDataPersistence>()
            .ToList();
    }

    #endregion

    public bool CanTPPlayerToLastPos() => 
        gameData.LastVisitedPuzzleId != null && string.IsNullOrEmpty(gameData.LastVisitedPuzzleId);
    
    public string GetLastVisitedPuzzleId() => gameData.LastVisitedPuzzleId;
}

