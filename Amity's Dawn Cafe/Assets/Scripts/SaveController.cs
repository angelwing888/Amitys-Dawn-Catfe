using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SaveController : MonoBehaviour
{
    /*
    private string saveLocation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // define save location
        
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        Debug.Log("Save file path: " + saveLocation);


        LoadGame();
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
        };
        
        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPosition;
        }
        else
        {
            SaveGame();
        }
    }
    */
}
