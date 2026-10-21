using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeMenu : MonoBehaviour
{
    public TextMeshProUGUI bankText;
    public TextMeshProUGUI UpgradeSpeedText;
    public TextMeshProUGUI UpgradeFortuneText;

    private bool speedUpgradePurchased;
    private bool fortuneUpgradePurchased;
    void Start()
    {
        bankText.text = "$"+Math.Floor( MoneyManager.Instance.GetBank() / 10) / 100 + "k";

        speedUpgradePurchased = SaveAndLoadManager.Instance.GetUpgrades("speed");
        fortuneUpgradePurchased = SaveAndLoadManager.Instance.GetUpgrades("fortune");
        
        ChangeTextColor();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void BuySpeedUpgrade()
    {
        if (speedUpgradePurchased) return;
        int cost = 1000;

        if (MoneyManager.Instance.GetBank() >= cost)
        {
            MoneyManager.Instance.AddMoneyToBank(-cost);
            speedUpgradePurchased = true;
            SaveAndLoadManager.Instance.PurchaseUpgrade("speed", cost);
            UpgradeSpeedText.color = Color.yellow;
        }
        bankText.text = "$"+Math.Floor( MoneyManager.Instance.GetBank() / 10) / 100 + "k";
    }

    public void BuyFortuneUpgrade()
    {
        if (fortuneUpgradePurchased) return;
        int cost = 1000;

        if (MoneyManager.Instance.GetBank() >= cost)
        {
            MoneyManager.Instance.AddMoneyToBank(-cost);
            fortuneUpgradePurchased = true;
            SaveAndLoadManager.Instance.PurchaseUpgrade("fortune", cost);
            UpgradeFortuneText.color = Color.yellow;
        }
        bankText.text = "$"+Math.Floor( MoneyManager.Instance.GetBank() / 10) / 100 + "k";
    }

    void ChangeTextColor()
    {
        if (speedUpgradePurchased)
        {
            UpgradeSpeedText.color = Color.yellow;
        }
        if (fortuneUpgradePurchased)
        {
            UpgradeFortuneText.color = Color.yellow;
        }
    }

}
