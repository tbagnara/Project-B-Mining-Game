using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    public TextMeshProUGUI leaderBoardText;
    void Start()
    {
        leaderBoardText.text = LeaderboardTracker.Instance.GetTopRuns();
        Time.timeScale = 1;
    }

    void Update()
    {
        
    }

    public void LoadScene(String sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
