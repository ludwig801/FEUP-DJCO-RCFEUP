using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    private int Speed = 1;
    private int Handling = 2;
    private int Weight = 3;

    public Text[] Values;

    public UpgradeLevelHandler[] LevelHandlers;

    void Start()
    {
        SetAttributeValues();
    }

    private void SetAttributeValues()
    {
        var minId = 1;
        var maxId = 3;

        for (int i = minId; i <= maxId; i++)
        {
            SetAttributeValueForId(i);
        }
    }

    private void SetAttributeValueForId(int upgradeId)
    {
        Values[upgradeId-1].text = "" + (100 + GetAttributeValue(upgradeId));
    }

    private int GetAttributeValue(int upgradeId)
    {
        int level = UpgradeReader.GetUpgradeLevel(upgradeId);

        return (new Upgrade() { UpgradeId = upgradeId, Level = level }).GetIncrementByLevel();
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
