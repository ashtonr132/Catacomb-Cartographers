using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class GameFiles : MonoBehaviour
{
    internal static SaveData saveData;
    internal SaveData[] Saves;
    const string folderName = "CatCar_SaveData", fileExtension = ".dat";
    private string dataPath;
    BinaryFormatter binaryFormatter;

    [SerializeField]
    GameObject Level;

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

    internal void SaveD(int? mainSave = null)
    {
        if (mainSave != null)
        {
            using (FileStream fileStream = File.Open(Path.Combine(dataPath, Saves[mainSave.Value].Tag + fileExtension), FileMode.OpenOrCreate))
            {
                binaryFormatter.Serialize(fileStream, Saves[mainSave.Value]);
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
                    Tag = "Save 1",
                },
                saveData = new SaveData
                {
                    Tag = "Save 2"
                },
                saveData = new SaveData
                {
                    Tag = "Save 3"
                }
            };
            for (int i = 0; i < Saves.Length; i++)
            {
                SaveD(i);
            }
        }

    }
    internal void setMainSave(int sD)
    {
        saveData = Saves[sD -1];
        transform.GetChild(0).GetComponent<Menu>().gameStarted();
        Level.GetComponent<LevelGen>().InitLevel(50 + saveData.Difficulty, saveData.Seed, saveData.Difficulty);
    }
}
[Serializable]
public class SaveData
{
    internal string Tag;
    internal int Difficulty = 1, Currency = 0;
    internal int? Seed = null;
    internal int[] Seeds;
}
