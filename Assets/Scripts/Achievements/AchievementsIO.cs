using System.IO;
using System.Xml;

public static class AchievementsIO
{
    public static void InitializeAchievementsNode(string filenameWithExtension)
    {
        using (var streamWriter = new StreamWriter(filenameWithExtension, true))
        {
            streamWriter.WriteLine(GetOpeningParentNode());

            streamWriter.WriteLine(GetChildNode(0));
            streamWriter.WriteLine(GetChildNode(1));
            streamWriter.WriteLine(GetChildNode(2));

            streamWriter.WriteLine(GetClosingParentNode());
        }
    }

    private static string GetOpeningParentNode()
    {
        return "    <Achievements>";
    }

    private static string GetClosingParentNode()
    {
        return "    </Achievements>";
    }

    private static string GetChildNode(int id)
    {
        return "        <Achievement id='" + id + "' status='0' />";
    }

    public static int GetAchievementStatus(int achievementId)
    {
        var xmlDoc = SaveGameFile.OpenSaveGameFile();

        var node = GetUpgradeNode(xmlDoc, achievementId);

        return int.Parse(node.Attributes[1].Value);
    }

    private static XmlNode GetUpgradeNode(XmlDocument xmlDoc, int achievementId)
    {
        var elements = xmlDoc.SelectSingleNode("/Document/Achievements").ChildNodes;

        foreach (XmlNode element in elements)
        {
            if (int.Parse(element.Attributes[0].Value) == achievementId)
            {
                return element;
            }
        }

        return null;

    }

    public static void ChangeAchievementStatus(int achievementId)
    {
        var xmlDoc = SaveGameFile.OpenSaveGameFile();
        var node = GetUpgradeNode(xmlDoc, achievementId);

        node.Attributes[1].Value = "" + (int.Parse(node.Attributes[1].Value) == 0 ? 1 : 0);

        xmlDoc.Save(SaveGameFile.Filename);
    }

}
