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

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
