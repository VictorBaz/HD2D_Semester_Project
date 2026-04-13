using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{

    [Header("Debbug")] 
    [SerializeField] private bool DisableSave = false;
    [SerializeField] private bool Init_Data_If_Null = false;
    [SerializeField] private bool overrideSelectedProfileID = false;
    [SerializeField] private string testSelectedProfile = "test";
    
    
    [Header("File Storage Config")] 
    [SerializeField] private string fileName; 
    [SerializeField] private bool useEncryption;
    
    private GameData gameData;
    
    private List<IDataPersistence> dataPersistencesObjects;
    private FileDataHandler dataHandler;

    private string SelectedProfile = "";
    
    public static DataPersistenceManager DataPM_instance { get; private set; }

    
    
    #region UnityLifeCycle

    private void Awake()
    {

        if (DataPM_instance != null)
        {
            Debug.LogWarning("There's more than one Data Persistence Manage in the scene ! Destroying the newest one !");
            Destroy(this.gameObject);
            return;
        }
        DataPM_instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (DisableSave)
        {
            Debug.LogWarning("Data persistence is currently disabled");
        }
        
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

        this.SelectedProfile = dataHandler.GetMostRecentlyUpdatedProfileID();

        if (overrideSelectedProfileID)
        {
            this.SelectedProfile = testSelectedProfile;
            Debug.LogWarning("Overrode selected profile id with test id : " + testSelectedProfile);
        }
        
    }
    
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    #endregion

    #region SceneLoad

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
        this.dataPersistencesObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void ChangeSelectedProfileID(string newProfileID)
    {
        this.SelectedProfile = newProfileID;
        
        LoadGame();
    }

    #endregion
    
    
    
    #region RelatedToSaveThing
    
    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {

        if (DisableSave)
        {
            return;
        }
        
        //Load any saved data from file using DataHandler
        this.gameData = dataHandler.Load(SelectedProfile);
        
        if (this.gameData == null && Init_Data_If_Null)
        {
            NewGame();
        }
        
        
        //if it don't work create a new one
        if (this.gameData == null)
        {
            Debug.Log("No data was found. A New need to be created first");
            return;
        }
        
        
        //push the loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistencesObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        
        if (DisableSave)
        {
            return;
        }

        if (this.gameData == null)
        {
            Debug.LogWarning("No Data was found . A New game need to be created first");
            return;
        }
        
        //Pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistencesObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        gameData.LastUpdated = System.DateTime.Now.ToBinary();
        
        //save that data to a file using the data handler
        dataHandler.Save(gameData,SelectedProfile);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {

        IEnumerable<IDataPersistence> dataPersistenceObjects =
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include,FindObjectsSortMode.None).OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfileGameData()
    {

        return dataHandler.LoadAllProfiles();

    }


    #endregion



}
