using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    private int Speed = 0;
    private int Handling = 1;
    private int Weight = 2;

    public UpgradeLevelHandler[] LevelHandlers;
    public Button[] UpgradeButtons;

    private Upgrade[] Upgrades;

    void Start()
    {
        SetUpgradeLevels();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void SetUpgradeLevels()
    {
        var minId = 0;
        var maxId = 2;

        Upgrades = new Upgrade[maxId + 1];

        for (int i = minId; i <= maxId; i++)
        {
            SetLevelForUpgradeWithId(i);
        }
    }

    private void SetLevelForUpgradeWithId(int upgradeId)
    {
        var currentLevel = GetLevel(upgradeId);
        Upgrades[upgradeId] = new Upgrade() { UpgradeId = upgradeId, Level = currentLevel };
        LevelHandlers[upgradeId].SetLevelTo(currentLevel);

        if (!Upgrades[upgradeId].CanIncrementLevel)
        {
            UpgradeButtons[upgradeId].gameObject.SetActive(false);
        }
    }

    private int GetLevel(int upgradeId)
    {
        return UpgradeWriter.GetUpgradeLevel(upgradeId);
    }

    public void UpgradeSpeed()
    {
        if (Upgrades[Speed].CanIncrementLevel)
        {
            Upgrades[Speed].Level++;
            UpgradeWriter.IncrementUpgradeLevel(Upgrades[Speed].UpgradeId);
            LevelHandlers[Speed].IncrementLevelIfNotMax();
        }

        if(!Upgrades[Speed].CanIncrementLevel)
        {
            UpgradeButtons[Speed].gameObject.SetActive(false);
        }
    }

    public void UpgradeHandling()
    {
        if (Upgrades[Handling].CanIncrementLevel)
        {
            Upgrades[Handling].Level++;
            UpgradeWriter.IncrementUpgradeLevel(Upgrades[Handling].UpgradeId);
            LevelHandlers[Handling].IncrementLevelIfNotMax();
        }

        if (!Upgrades[Handling].CanIncrementLevel)
        {
            UpgradeButtons[Handling].gameObject.SetActive(false);
        }
    }

    public void UpgradeWeight()
    {
        if(Upgrades[Weight].CanIncrementLevel)
        {
            Upgrades[Weight].Level++;
            UpgradeWriter.IncrementUpgradeLevel(Upgrades[Weight].UpgradeId);
            LevelHandlers[Weight].IncrementLevelIfNotMax();
        }

        if (!Upgrades[Weight].CanIncrementLevel)
        {
            UpgradeButtons[Weight].gameObject.SetActive(false);
        }
    }

}
