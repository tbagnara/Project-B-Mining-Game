using System;
using System.Collections;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    [SerializeField] private Canvas pauseMenu;
    [SerializeField] private Canvas recordMenu;
    [SerializeField] private TextMeshProUGUI walletText;
    [SerializeField] private TextMeshProUGUI menuText;
    [SerializeField] private TMP_InputField recordName;   // Input field text 
    public float wallet;
    float runTime;
    public Boolean gameEnded;
    private int fortune = 1;

    void Awake()
    {
        if (Instance != null && Instance !=this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        TimerManager.Instance.OnRunFinished += EndRun;
    }

    void OnDisable()
    {
        TimerManager.Instance.OnRunFinished -= EndRun;
    }

    void Start()
    {
        walletText.text = "$0.00";
        if (SaveAndLoadManager.Instance.GetUpgrades("fortune"))
        {
            fortune = 2;
        }
    }

    void Update()
    {
        OpenPauseMenu();
    }

    void EndRun(float time)
    {
        runTime = time;
        gameEnded = true;
        MoneyManager.Instance.AddMoneyToBank(wallet);
        if ( LeaderboardTracker.Instance.IsRecordRun(wallet) )   // Is it in the top 5 runs
        {
            recordMenu.enabled = true;
        }
        else    // Slower time = no time recorded
        {

            DisplayPauseMenu("PoorRecord");
            StartCoroutine(WaitTimeCoroutine(2f));
            
        }
        
    }
    public void OreMined(String ore)
    {
        float price = 0;
        switch (ore)
        {
            case "MiningTextures2_1":
            {
                price = 10;
                break;
            }
            case "MiningTextures2_2":
            {
                price = 20;
                break;
            }
            case "MiningTextures2_3":
            {
                price = 30;
                break;
            }
            case "MiningTextures2_4":
            {
                price = 40;
                break;
            }
            case "MiningTextures2_5":
            {
                price = 50;
                break;
            }
            case "MiningTextures2_6":
            {
                price = 60;
                break;
            }
            default:
                {
                    return;
                }
        }
        price *= fortune;
        CheckForTimeBonus(wallet + price);
        wallet += price;
        walletText.text = "$" +wallet;
    }
    void CheckForTimeBonus(float newAmount)
    {
        if ( Math.Floor(newAmount / 10000) > Math.Floor(wallet / 10000) )
        {
            TimerManager.Instance.AddTime(60);
        }
        else if ( Math.Floor(newAmount / 5000) > Math.Floor(wallet / 5000) )
        {
            TimerManager.Instance.AddTime(20);
        }
        else if ( Math.Floor(newAmount / 1000) > Math.Floor(wallet / 1000) )
        {
            TimerManager.Instance.AddTime(10);
        }
        
    }

    void OpenPauseMenu()
    {
        if (gameEnded) return;
        if ( Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace) )
        {
            if (pauseMenu.isActiveAndEnabled == false) 
            {
                DisplayPauseMenu("Pause");
                Time.timeScale = 0;
            }
            else
            {
                DisplayPauseMenu("Hide");
                Time.timeScale = 1;
            }
        }
    }

    public void ExitRun()
    {
        TimerManager.Instance.EndRun();
    }

    void DisplayPauseMenu(String s)    // Brings UI (pauseMenu) ontoscreen, changes text based on which menu is in use
    {

        switch (s) 
        {
            case "Pause": 
                pauseMenu.transform.position = new UnityEngine.Vector3(0, 0, -1);
                break;
            case "PoorRecord": // Won but not record time
                menuText.text = "- Time's Up -";
                break;
            default:
                pauseMenu.enabled = false;
                return;
        }
        pauseMenu.enabled = true;
    }

    public void RecordPlayerName() // Triggered when input field is exited
    {
        
        string playerName = recordName.text;
        if (playerName.Length > 3) 
        {
            playerName = playerName.Substring(0,3);
        }
        LeaderboardTracker.Instance.SaveRunScore(playerName, wallet, runTime);
        SceneManager.LoadScene("MainMenu");
    }
    
    IEnumerator WaitTimeCoroutine(float seconds)
    {

        yield return new WaitForSecondsRealtime(seconds);
        SceneManager.LoadScene("MainMenu");

    }

    public Boolean IsGameOver()
    {
        return gameEnded;
    }
}
