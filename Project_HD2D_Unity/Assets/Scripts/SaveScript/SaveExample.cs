using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class SaveExample : MonoBehaviour, IDataPersistence
{
    #region Variables

    [SerializeField] private int ParasiteID = 0;
    
    private int MagicPoint = 10;
    private int MagicPointLimit = 10;
    
    private List<bool> ParasiteAliveStates = new List<bool>();
    
    [Header("Save folder etc ...")]
    [SerializeField] private string debugDirectoryName = "Debug/Debug";
    [SerializeField] private string debugFileName = "debug_save.json";
    [SerializeField] private int SaveStateID = 0;
    
    #endregion

    #region Link

    public TMP_Text TestText;

    #endregion

    #region UnityLifeCycle

    private void Start()
    {
        UpdateText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddPoint();
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            RemovePoint();
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            KillParasite(ParasiteID);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            MakeASaveState();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadASaveState();
        }
    }

    #endregion

    #region TestingFunctions

    private void AddPoint()
    {
        if (MagicPoint + 1 <= MagicPointLimit)
        {
            MagicPoint++;
        }
        UpdateText();
    }
    
    private void RemovePoint()
    {
        if (MagicPoint - 1 >= 0)
        {
            MagicPoint--;
        }
        UpdateText();
    }

    public void KillParasite(int parasiteIndex)
    {
        if (parasiteIndex >= 0 && parasiteIndex < ParasiteAliveStates.Count)
        {
            ParasiteAliveStates[parasiteIndex] = false;
        }
    }

    public void MakeASaveState()
    {
        
        string fullPath = Path.Combine(Application.persistentDataPath, debugDirectoryName);
        
        FileDataHandler debugHandler = new FileDataHandler(fullPath, debugFileName, false);
        
        GameData dataToSave = new GameData();
        SaveData(ref dataToSave);
        
        string profileID = "State_" + SaveStateID;
        debugHandler.Save(dataToSave, profileID);

        Debug.Log($"<color=green>Save State créé pour l'ID {SaveStateID} dans {fullPath}/{profileID}</color>");
    }
    
    public void LoadASaveState()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, debugDirectoryName);
        
        FileDataHandler debugHandler = new FileDataHandler(fullPath, debugFileName, false);
        
        string profileID = "State_" + SaveStateID;
        GameData loadedData = debugHandler.Load(profileID);
        
        if (loadedData != null)
        {
            LoadData(loadedData);
            UpdateText();
            Debug.Log($"<color=cyan>Save State chargé pour l'ID {SaveStateID}</color>");
        }
        else
        {
            Debug.LogWarning("Aucune sauvegarde de debug trouvée pour l'ID : " + SaveStateID);
        }
    }

    #endregion

    private void UpdateText()
    {
        TestText.text = "" + MagicPoint;
    }

    #region SaveThing

    public void LoadData(GameData data)
    {
        this.MagicPoint = data.EnergyPoint;
        this.ParasiteAliveStates = new List<bool>(data.ParasitesAlive);
    }

    public void SaveData(ref GameData data)
    {
        data.EnergyPoint = this.MagicPoint;
        data.ParasitesAlive = new List<bool>(this.ParasiteAliveStates);
    }

    #endregion
}