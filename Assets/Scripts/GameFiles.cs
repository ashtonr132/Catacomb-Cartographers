using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour
{
    internal static SaveData saveData;
    internal SaveData[] Saves;
    const string folderName = "CatCar_SaveData", fileExtension = ".dat";
    private string dataPath;
    BinaryFormatter binaryFormatter;

    // Start is called before the first frame update
    void Start()
    {
        binaryFormatter = new BinaryFormatter();
        dataPath = Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }
        LoadD();
    }

    internal void SaveD()
    {
        for (int i = 0; i < Saves.Length; i++)
        {
            if (Saves[i].name == saveData.name)
            {
                using (FileStream fileStream = File.Open(Path.Combine(dataPath, saveData.name + fileExtension), FileMode.OpenOrCreate))
                {
                    binaryFormatter.Serialize(fileStream, saveData);
                }
            }
        }
        
    }

    internal void LoadD()
    {
        string[] filePaths = Directory.GetFiles(dataPath, fileExtension);
        if (filePaths.Length > 0)
        {
            Saves = new SaveData[filePaths.Length];
            for (int i = 0; i < filePaths.Length; i++)
            {
                using (FileStream fileStream = File.Open(filePaths[i], FileMode.Open))
                {
                    Saves[i] = (SaveData)binaryFormatter.Deserialize(fileStream);
                }
            }
        }
        else
        {
            Saves = new SaveData[] 
            {
                saveData = new SaveData
                {
                    name = "Save 1"
                },
                saveData = new SaveData
                {
                    name = "Save 2"
                },
                saveData = new SaveData
                {
                    name = "Save 3"
                }
            };
        }
    }
}
public class SaveData
{
    internal string name, tag;
    internal int difficulty, currency;
}
