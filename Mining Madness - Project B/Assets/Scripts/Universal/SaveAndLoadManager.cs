// Saving to JSON file code is from Rize Education class GDM4 - C# programming

using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;


[System.Serializable]
public class PlayerSaveData
{
    public List<RunHighScore> leaderBoard;
    public float wealth;
    public string lastPlayed;

}

public class SaveAndLoadManager : MonoBehaviour
{
    public static SaveAndLoadManager Instance { get; private set; }
    
    private PlayerSaveData playerData;
    private string savePath;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        savePath = Path.Combine(Application.persistentDataPath, "playersave.json");
        LoadPlayerData();

        MoneyManager.Instance.SetBank( playerData.wealth );
        
        if (playerData.leaderBoard.Count != 0 )
            LeaderboardTracker.Instance.LoadLeaderBoard( playerData.leaderBoard);
    }
    public void SavePlayerData()
    {
        playerData.lastPlayed = System.DateTime.Now.ToString();
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(savePath, json);
        Application.ExternalEval("_JS_FileSystem_Sync();");
        Debug.Log("Player data saved!");
    }
    
    public void LoadPlayerData()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                playerData = JsonUtility.FromJson<PlayerSaveData>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Load failed: " + e.Message);
                CreateNewPlayerData();
            }
        }
        else
        {
            CreateNewPlayerData();
        }
    }
    
    void CreateNewPlayerData()      // Create savedata
    {
        playerData = new PlayerSaveData();
        playerData.wealth = 0;
        playerData.leaderBoard = new List<RunHighScore>();
        SavePlayerData();
    }
    
    public void SaveData(List<RunHighScore> lb, float wealth)
    {
        playerData.leaderBoard = new List<RunHighScore>(lb);
        playerData.wealth = wealth;

        SavePlayerData();
    }

    void OnApplicationQuit()
    {
        SavePlayerData();
    }

}