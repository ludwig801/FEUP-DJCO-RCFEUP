using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    private int Speed = 1;
    private int Handling = 2;
    private int Weight = 3;

    public Text[] Values;

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
        Values[0].text = "" + (100 + GetAttributeValue(1));
    }

    public void UpgradeHandling()
    {
        UpgradeWriter.Save(new Upgrade() { UpgradeId = Handling });
        Values[1].text = "" + (100 + GetAttributeValue(2));
    }

    public void UpgradeWeight()
    {
        UpgradeWriter.Save(new Upgrade() { UpgradeId = Weight });
        Values[2].text = "" + (100 + GetAttributeValue(3));
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
