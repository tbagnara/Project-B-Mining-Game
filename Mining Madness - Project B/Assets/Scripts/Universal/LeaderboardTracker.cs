using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using System.Linq;
using UnityEditor;
using UnityEngine.SocialPlatforms.Impl;

[System.Serializable]
public class RunHighScore
{
    public String name;
    public float time;
    public float money;
    public RunHighScore(String n, float m, float t)
    {
        name = n;
        money = m;
        time = t;
    }
}
public class LeaderboardTracker : MonoBehaviour
{
    public static LeaderboardTracker Instance {get; private set;}
    List<RunHighScore> leaderBoard = new List<RunHighScore>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void SaveRunScore(string name, float money, float time)
    {
        leaderBoard.Add(new RunHighScore(name, money, time));
        leaderBoard = leaderBoard.OrderByDescending(hs => hs.money).ToList();
        SaveAndLoadManager.Instance.SaveDataFromRun(leaderBoard, MoneyManager.Instance.GetBank() );
    }


    public Boolean IsRecordRun(float m)
    {

        if (leaderBoard.Count < 5 || m > leaderBoard[4].money) return true;
        return false;
        
    }

    public String GetTopRuns()
    {
        String runRecords;
        String[] records = {"- - - - - - - -","- - - - - - - -","- - - - - - - -","- - - - - - - -","- - - - - - - -"};
        
        for (int i = 0; i<5; i++)
        {
            try
            {
                if (leaderBoard[i] != null)
                {
                    records[i] = ""+(i+1)+". " + leaderBoard[i].name + " $" + Math.Floor(leaderBoard[i].money / 10) / 100 +"k";
                }
            }
            catch
            {
                
            }
        }
        
        runRecords =    "_Leaderboard_" + '\n' +
                        records[0] + '\n'+
                        records[1] + '\n'+
                        records[2] + '\n'+ 
                        records[3] + '\n'+
                        records[4];
        return runRecords;
    }

    public void LoadLeaderBoard(List<RunHighScore> lb)
    {
        leaderBoard = lb;
    }

}
