using System;
using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance {get; private set;}
    public event Action<float> OnRunFinished;
    public event Action<Boolean> OnRunStarted;
    public float runStartLength;
    public float endTime;
    public float startTime;
    [SerializeField] private TextMeshProUGUI remainingTimeText;
    float remainingTime;
    Boolean over;
    void Awake()
    {
        if (Instance != null && Instance !=this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        startTime = Time.time;
        endTime = startTime + runStartLength; // 60 seconds after startTime
        remainingTimeText.text = "60";
        OnRunStarted?.Invoke(true);
    }


    void Update()
    {
        IsTimerOver();
        UpdateTimerText();
    }

    void IsTimerOver()
    {
        if (Time.time>=endTime && !over)
        {
            over = true;
            Time.timeScale = 0;
            OnRunFinished?.Invoke(endTime-startTime);
        }
    }
    void UpdateTimerText()
    {
        remainingTime = endTime - Time.time;
        if (remainingTime > 0)
        {
            remainingTimeText.text = ""+Math.Ceiling(remainingTime);
        }
        else
        {
            remainingTimeText.text = "0";
        }
        
    }

    public void AddTime(float f)
    {
        endTime += f;
    }

}
