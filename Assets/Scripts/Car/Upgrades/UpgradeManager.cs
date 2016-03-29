using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    private int Speed = 0;
    private int Handling = 1;
    private int Weight = 2;

    public Text CoinsValue;
    public UpgradeLevelHandler[] LevelHandlers;
    public UpgradeUI[] UpgradeButtons;

    [SerializeField]
    private List<Upgrade> Upgrades;
    private int NumCoins;

    void Start()
    {
        NumCoins = CoinsIO.GetCoinCount();
        ReadCurrentUpgrades();
        UpdateUIValues();
        SaveValuesToFile();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("MainMenu");
    }

    private void ReadCurrentUpgrades()
    {
        var numUpgrades = 3;
        Upgrades = new List<Upgrade>();

        for (int i = 0; i < numUpgrades; i++)
        {
            var upgrade = new Upgrade();
            upgrade.UpgradeId = i;
            upgrade.Level = UpgradeWriter.GetUpgradeLevel(i);
            LevelHandlers[i].SetLevelTo(upgrade.Level);
            Upgrades.Add(upgrade);
        }
    }

    private void UpdateUIValues()
    {
        for (int i = 0; i < Upgrades.Count; i++)
        {
            var upgrade = Upgrades[i];
            Debug.Log(i + " | " + upgrade.Cost);
            UpgradeButtons[i].Btn.interactable = upgrade.CanIncrementLevel && NumCoins >= upgrade.Cost;
            UpgradeButtons[i].Value.text = upgrade.Cost.ToString();
            LevelHandlers[i].SetLevelTo(upgrade.Level);
        }

        CoinsValue.text = NumCoins.ToString();
    }

    private void SaveValuesToFile()
    {
        CoinsIO.SetCoinCount(NumCoins);
        for (int i = 0; i < Upgrades.Count; i++)
        {
            var upgrade = Upgrades[i];
            Debug.Log(i + " | " + upgrade.Level);
            UpgradeWriter.SetUpgradeLevel(i, upgrade.Level);
        }
    }

    public void UpgradeSpeed()
    {
        UpgradeLevel(Speed);
    }

    public void UpgradeHandling()
    {
        UpgradeLevel(Handling);
    }

    public void UpgradeWeight()
    {
        UpgradeLevel(Weight);
    }

    private void UpgradeLevel(int index)
    {
        var upgrade = Upgrades[index];
        if (upgrade.Cost <= NumCoins && upgrade.CanIncrementLevel)
        {
            NumCoins -= upgrade.Cost;
            upgrade.Level++;
        }

        UpdateUIValues();
        SaveValuesToFile();
    }
}

[System.Serializable]
public class UpgradeUI
{
    public Button Btn;
    public Text Value;
}