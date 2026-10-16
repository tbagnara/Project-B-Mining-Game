using UnityEngine;

public class MoneyManager : MonoBehaviour
{

    public static MoneyManager Instance {get; private set;}
    public float bank;    

    void Awake()
    {
        if (Instance != null && Instance !=this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void AddMoneyToBank(float wallet)
    {
        bank += wallet;
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
