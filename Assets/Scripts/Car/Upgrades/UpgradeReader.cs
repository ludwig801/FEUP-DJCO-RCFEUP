using System.Xml;

public static class UpgradeReader
{
    private static string Filename = "Upgrades.sav";

    public static int GetUpgradeLevel(int upgradeId)
    {
        var xml = new XmlDocument();

        xml.Load(Filename);

        var upgradesNode = xml.SelectSingleNode("/Upgrades").ChildNodes;

        foreach (XmlNode upgrade in upgradesNode)
        {
            if (int.Parse(upgrade.Attributes[0].Value) == upgradeId)
            {
                return int.Parse(upgrade.Attributes[1].Value);
            }
        }

        return -1;
    }

}
