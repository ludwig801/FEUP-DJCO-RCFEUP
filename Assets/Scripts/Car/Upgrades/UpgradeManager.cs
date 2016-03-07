using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    private int Speed = 1;
    private int Handling = 2;
    private int Weight = 3;

    public UpgradeLevelHandler[] LevelHandlers;

    void Start()
    {
        SetUpgradeLevels();
    }

    private void SetUpgradeLevels()
    {
        var minId = 1;
        var maxId = 3;

        for (int i = minId; i <= maxId; i++)
        {
            SetLevelForUpgradeWithId(i);
        }
    }

    private void SetLevelForUpgradeWithId(int upgradeId)
    {
        LevelHandlers[upgradeId - 1].SetLevelTo(GetLevel(upgradeId));
    }

    private int GetLevel(int upgradeId)
    {
        return UpgradeReader.GetUpgradeLevel(upgradeId);
    }

    public void UpgradeSpeed()
    {
        UpgradeWriter.Save(new Upgrade() { UpgradeId = Speed });
        LevelHandlers[0].IncrementLevelIfNotMax();
    }

    public void UpgradeHandling()
    {
        UpgradeWriter.Save(new Upgrade() { UpgradeId = Handling });
        LevelHandlers[1].IncrementLevelIfNotMax();
    }

    public void UpgradeWeight()
    {
        UpgradeWriter.Save(new Upgrade() { UpgradeId = Weight });
        LevelHandlers[2].IncrementLevelIfNotMax();
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
