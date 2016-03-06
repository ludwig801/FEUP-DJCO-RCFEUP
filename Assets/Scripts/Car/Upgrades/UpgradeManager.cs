using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    private int Speed = 1;
    private int Handling = 2;
    private int Weight = 3;

    void Start()
    {
        SetAttributeValues();
    }

    private void SetAttributeValues()
    {
        var minId = 1;
        var maxId = 3;

        for(int i = minId; i <= maxId; i++)
        {
            SetAttributeValueForId(i);
        }
    }

    private void SetAttributeValueForId(int upgradeId)
    {
        string[] attributesNames = { "SpeedAttribute", "HandlingAttribute", "WeightAttribute" };

        var attributeObj = GameObject.Find(attributesNames[upgradeId - 1]);

        var childrens = attributeObj.GetComponentsInChildren<Text>();

        foreach (var item in childrens)
        {
            if (item.name == "AttributeValue")
            {
                item.text = "" + (100 + GetAttributeValue(upgradeId));
            }
        }
    }

    private int GetAttributeValue(int upgradeId)
    {
        int level = UpgradeReader.GetUpgradeLevel(upgradeId);

        return (new Upgrade() { UpgradeId = upgradeId, Level = level }).GetIncrementByLevel();
    }

    public void UpgradeSpeed()
    {
        UpgradeWriter.Save(new Upgrade() { UpgradeId = Speed });
    }

    public void UpgradeHandling()
    {
        UpgradeWriter.Save(new Upgrade() { UpgradeId = Handling });
    }

    public void UpgradeWeight()
    {
        UpgradeWriter.Save(new Upgrade() { UpgradeId = Weight });
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
