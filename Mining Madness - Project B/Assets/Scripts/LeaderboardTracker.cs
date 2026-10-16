using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Node 
{
    public RunHighScore Data { get; set; }
    public Node Next { get; set; }

    public Node(RunHighScore data) 
    {
        Data = data;
        Next = null; // Next is null by default
    }
}
public class RunHighScore
{
    public String name {get; private set;}
    public float time {get; private set;}
    public float money {get; private set;}
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
    Node leaderBoardHead = null;

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
        Node newValue = new Node(new RunHighScore(name, money, time));
        SaveRunScoreHelper(leaderBoardHead, newValue);
    }

    public void SaveRunScoreHelper(Node current, Node newValue)
    {
        if (leaderBoardHead == null) // Empty leaderboard
        {
            leaderBoardHead = newValue;
        }
        else if (current.Data == leaderBoardHead.Data && newValue.Data.money > leaderBoardHead.Data.money) // New time isthe best
        {
            newValue.Next = leaderBoardHead;
            leaderBoardHead = newValue;
        }
        else if (current.Next == null)
        {
            current.Next = newValue;
        }
        else if (current.Next.Data.money > newValue.Data.money) // If the next one is greater
        {
            SaveRunScoreHelper(current.Next, newValue);
        }
        else 
        {
            newValue.Next = current.Next;
            current.Next = newValue;
        }

    }
    public Boolean IsRecordRun(float m)
    {
        Node current = leaderBoardHead;
        try
        {
            current = current.Next.Next.Next.Next;
            if (m > current.Data.money) return true;
            return false;
        }
        catch
        {
            return true;
        }
        
        
    }

    public String GetTopRuns()
    {
        String runRecords;
        Node current = leaderBoardHead;
        String[] records = new String[5];
        for (int i = 0; i<5; i++)
        {
            if (current == null) 
            {
                records[i] = "- - - - - - - -";
            }
            else
            {
                records[i] = ""+ (i+1) +". "+current.Data.name + " $" + Math.Floor(current.Data.money*100) / 100 ;
                current = current.Next;
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

}
