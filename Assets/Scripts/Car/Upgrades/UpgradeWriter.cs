using System;
using System.IO;
using System.Xml;
using UnityEngine;

public static class UpgradeWriter
{
    public static void InitializeUpgradesNode(string filenameWithExtension)
    {
        using (var streamWriter = new StreamWriter(filenameWithExtension, true))
        {
            streamWriter.WriteLine(GetOpeningParentNode());

            streamWriter.WriteLine(GetChildNode(0, 0));
            streamWriter.WriteLine(GetChildNode(1, 0));
            streamWriter.WriteLine(GetChildNode(2, 0));

            streamWriter.WriteLine(GetClosingParentNode());
        }
    }

    private static string GetOpeningParentNode()
    {
        return "    <Upgrades>";
    }

    private static string GetClosingParentNode()
    {
        return "    </Upgrades>";
    }

    private static string GetChildNode(int id, int level)
    {
        return "        <Upgrade id='" + id + "' level='" + level + "' />";
    }

    public static void IncrementUpgradeLevel(int upgradeId)
    {
        var xmlDoc = SaveGameFile.OpenSaveGameFile();

        var node = GetUpgradeNode(xmlDoc, upgradeId);

        node.Attributes[1].Value = "" + (int.Parse(node.Attributes[1].Value) + 1);

        xmlDoc.Save(SaveGameFile.Filename);
    }

    public static void SetUpgradeLevel(int upgradeId, int newValue)
    {
        var xmlDoc = SaveGameFile.OpenSaveGameFile();
        var node = GetUpgradeNode(xmlDoc, upgradeId);

        node.Attributes[1].Value = newValue.ToString();
        xmlDoc.Save(SaveGameFile.Filename);
    }

    public static int GetUpgradeLevel(int upgradeId)
    {
        var xmlDoc = SaveGameFile.OpenSaveGameFile();

        var node = GetUpgradeNode(xmlDoc, upgradeId);

        return int.Parse(node.Attributes[1].Value);
    }

    private static XmlNode GetUpgradeNode(XmlDocument xmlDoc, int upgradeId)
    {
        var elements = xmlDoc.SelectSingleNode("/Document/Upgrades").ChildNodes;

        foreach (XmlNode element in elements)
        {
            if (int.Parse(element.Attributes[0].Value) == upgradeId)
            {
                return element;
            }
        }

        return null;

    }
}
