using UnityEngine;

public class MoneyManager : MonoBehaviour
{

    public static MoneyManager Instance {get; private set;}
    private float bank;    

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

    public void AddMoneyToBank(float amount)
    {
        bank += amount;
    }

    public void SetBank(float m)
    {
        bank = m;
    }

    public float GetBank()
    {
        return bank;
    }
    

}
