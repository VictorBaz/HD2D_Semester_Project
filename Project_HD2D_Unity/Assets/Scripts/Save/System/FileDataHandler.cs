using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    #region Variables

    private string dataDirPath = "";
    private string dataFileName = "";

    private bool useEncryption = false;
    private readonly string encryptionCode = "polycount";

    #endregion

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;

        if (!File.Exists(fullPath)) return null;

        try
        {
            string dataToLoad;
            using (FileStream stream = new FileStream(fullPath, FileMode.Open))
            using (StreamReader reader = new StreamReader(stream))
            {
                dataToLoad = reader.ReadToEnd();
            }

            if (useEncryption)
                dataToLoad = EncryptDecrypt(dataToLoad);

            loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
        }
        catch (Exception e)
        {
            Debug.LogError("ERROR TRYING TO LOAD DATA FROM FILE: " + fullPath + "\n" + e);
        }

        return loadedData;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            if (useEncryption)
                dataToStore = EncryptDecrypt(dataToStore);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(dataToStore);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("ERROR TRYING TO SAVE DATA TO FILE: " + fullPath + "\n" + e);
        }
    }

    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
            modifiedData += (char)(data[i] ^ encryptionCode[i % encryptionCode.Length]);

        return modifiedData;
    }
}