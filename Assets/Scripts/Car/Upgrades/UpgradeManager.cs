using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    private int Speed = 0;
    private int Handling = 1;
    private int Weight = 2;

    public UpgradeLevelHandler[] LevelHandlers;

    private Upgrade[] Upgrades;

    void Start()
    {
        SetUpgradeLevels();
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
    }

    private int GetLevel(int upgradeId)
    {
        return UpgradeReader.GetUpgradeLevel(upgradeId);
    }

    public void UpgradeSpeed()
    {
        if (Upgrades[Speed].CanIncrementLevel)
        {
            Upgrades[Speed].Level++;
            UpgradeWriter.Save(Upgrades[Speed]);
            LevelHandlers[Speed].IncrementLevelIfNotMax();
        }
    }

    public void UpgradeHandling()
    {
        if (Upgrades[Handling].CanIncrementLevel)
        {
            Upgrades[Handling].Level++;
            UpgradeWriter.Save(Upgrades[Handling]);
            LevelHandlers[Handling].IncrementLevelIfNotMax();
        }
    }

    public void UpgradeWeight()
    {
        if(Upgrades[Weight].CanIncrementLevel)
        {
            Upgrades[Weight].Level++;
            UpgradeWriter.Save(Upgrades[Weight]);
            LevelHandlers[Weight].IncrementLevelIfNotMax();
        }
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
