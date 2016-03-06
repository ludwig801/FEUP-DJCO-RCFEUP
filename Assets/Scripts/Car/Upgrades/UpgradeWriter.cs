using System;
using System.IO;
using System.Xml;
using UnityEngine;

public static class UpgradeWriter
{
    private static string Filename = "Upgrades.sav";

    public static void Save(Upgrade upgrade)
    {

        if(!FileExists(Filename))
        {
            InitializeUpgradesFile(Filename);
        }

        SaveUpgrade(upgrade);
    }

    private static bool FileExists(string filenameWithExtension)
    {
        return File.Exists(filenameWithExtension);
    }

    private static void InitializeUpgradesFile(string filenameWithExtension)
    {
        using (var streamWriter = new StreamWriter(filenameWithExtension))
        {
            streamWriter.WriteLine("<?xml version='1.0' encoding='utf - 8'?>");

            streamWriter.WriteLine("<Upgrades>");

            streamWriter.WriteLine("    <Upgrade id='" + 1 + "' level='" + 0 + "' />");
            streamWriter.WriteLine("    <Upgrade id='" + 2 + "' level='" + 0 + "' />");
            streamWriter.WriteLine("    <Upgrade id='" + 3 + "' level='" + 0 + "' />");

            streamWriter.WriteLine("</Upgrades>");
        }
    }

    private static void SaveUpgrade(Upgrade upgrade)
    {
        IncrementLevelInLine(upgrade.UpgradeId);
    }

    private static void IncrementLevelInLine(int upgradeId)
    {
        var xml = new XmlDocument();

        xml.Load(Filename);

        var elements = xml.SelectSingleNode("/Upgrades").ChildNodes;

        foreach (XmlNode element in elements)
        {
            if (int.Parse(element.Attributes[0].Value) == upgradeId)
            {
                element.Attributes[1].Value = "" + (int.Parse(element.Attributes[1].Value) + 1);
            }
        }

        xml.Save(Filename);

    }
}

