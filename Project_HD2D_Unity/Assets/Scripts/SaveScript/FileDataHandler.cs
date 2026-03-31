using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class FileDataHandler 
{

    #region Variables

    private string dataDirPath = "";
    private string dataFileName = "";

    private bool useEncryption = false;
    private readonly string encryptionCode = "polycount";

    #endregion
    
    public FileDataHandler(string dataDirPath, string dataFileName,bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    public GameData Load(string profileID)
    {
        if (profileID == null)
        {
            return null;
        }
        
        
        // Using Path.combine to prevent error on other OS that don't use Windows
        string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                //Load the file (JSON)
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath,FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }
                
                // Dark Magic to go from JSON to C#
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);

            }
            catch (Exception e)
            {
                Debug.LogError(" ERROR TRYING TO LOAD DATA FROM THE FILE !!!" + fullPath + "\n" + e);
            }
        }

        return loadedData;
    }

    public void Save(GameData data, string profileID)
    {
        
        if (profileID == null)
        {
            return;
        }
        
        // Using Path.combine to prevent error on other OS that don't use Windows
        string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
        try
        {
            // create the directory where we will save to , if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            
            // Dark magic to convert C# data to JSON
            string dataToStore = JsonUtility.ToJson(data, true);

            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }
            
            // The writer
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(" ERROR TRYING TO SAVE DATA TO FILE !!!" + fullPath + "\n" + e);
        }
    }

    public Dictionary<string, GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();

        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileID = dirInfo.Name;

            string fullpath = Path.Combine(dataDirPath, profileID, dataFileName);
            if (!File.Exists(fullpath))
            {
                Debug.LogWarning("Skipping directory when loading all profiles bc it doesn't have data in it : " + profileID);
                continue;
            }

            GameData profileData = Load(profileID);
            if (profileData != null)
            {
                profileDictionary.Add(profileID,profileData);
            }
            else
            {
                Debug.LogError("Tried to load profile , but something went wrong ! ProfileID : " + profileID);
                
            }
            
            
        }
        
        return profileDictionary;
    }

    public string GetMostRecentlyUpdatedProfileID()
    {
        string mostRecentProfileID = null;
        Dictionary<string, GameData> profilesGameData = LoadAllProfiles();
        foreach (KeyValuePair<string,GameData> pair in profilesGameData)
        {
            string profileID = pair.Key;
            GameData gameData = pair.Value;

            if (gameData == null)
            {
                continue;
            }

            if (mostRecentProfileID == null)
            {
                mostRecentProfileID = profileID;
            }
            else
            {
                DateTime mostRecentDateTime = DateTime.FromBinary(profilesGameData[mostRecentProfileID].LastUpdated);
                DateTime newDateTime = DateTime.FromBinary(gameData.LastUpdated);

                if (newDateTime > mostRecentDateTime)
                {
                    mostRecentProfileID = profileID;
                }
                
            }
            
        }

        return mostRecentProfileID;
    }

    // XOR encryption
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCode[i % encryptionCode.Length]);
        }

        return modifiedData;
    }
}
