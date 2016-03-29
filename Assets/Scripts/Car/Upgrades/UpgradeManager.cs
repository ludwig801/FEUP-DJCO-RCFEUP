using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    private int Speed = 0;
    private int Handling = 1;
    private int Weight = 2;

    public UpgradeLevelHandler[] LevelHandlers;
    public UpgradeUI[] UpgradeButtons;

    private List<Upgrade> Upgrades;

    void Start()
    {
        SetUpgradeLevels();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("MainMenu");

        for (int i = 0; i < Upgrades.Count; i++)
        {

        }
    }

    private void SetUpgradeLevels()
    {
        var minId = 0;
        var maxId = 2;

        Upgrades = new List<Upgrade>();

        for (int i = minId; i <= maxId; i++)
            SetLevelForUpgradeWithId(i);
    }

    private void SetLevelForUpgradeWithId(int upgradeId)
    {
        var currentLevel = GetLevel(upgradeId);
        Upgrades.Add(new Upgrade() { UpgradeId = upgradeId, Level = currentLevel });
        LevelHandlers[upgradeId].SetLevelTo(currentLevel);

        UpgradeLevel(upgradeId, currentLevel);
    }

    private int GetLevel(int upgradeId)
    {
        return UpgradeWriter.GetUpgradeLevel(upgradeId);
    }

    public void UpgradeSpeed()
    {
        UpgradeLevel(Speed, Upgrades[Speed].Level);
    }

    public void UpgradeHandling()
    {
        UpgradeLevel(Handling, Upgrades[Handling].Level);
    }

    public void UpgradeWeight()
    {
        UpgradeLevel(Weight, Upgrades[Weight].Level);
    }

    private void UpgradeLevel(int index, int currentLevel)
    {
        if (Upgrades[index].CanIncrementLevel)
        {
            Upgrades[index].Level++;
            UpgradeWriter.IncrementUpgradeLevel(Upgrades[index].UpgradeId);
            LevelHandlers[index].IncrementLevelIfNotMax();
        }

        if (!Upgrades[index].CanIncrementLevel)
        {
            UpgradeButtons[index].Btn.interactable = false;
            UpgradeButtons[index].Value.text = string.Empty;
        }
        else
        {
            UpgradeButtons[index].Btn.interactable = true;
            UpgradeButtons[index].Value.text = string.Concat((currentLevel + 1) * 100);
        }
    }
}

[System.Serializable]
public class UpgradeUI
{
    public Button Btn;
    public Text Value;
}