using System.IO;
using System.Xml;

public static class SaveGameFile {

    public const string Filename = "savegame.sav";

    public static XmlDocument OpenSaveGameFile()
    {
        if(!File.Exists(Filename))
        {
            CreateSaveGameFile();
        }

        var doc = new XmlDocument();
        doc.Load(Filename);

        return doc;
    }

    private static void CreateSaveGameFile()
    {
        using (var streamWriter = new StreamWriter(Filename, true))
        {
            streamWriter.WriteLine(GetXmlVersion());
            streamWriter.WriteLine(GetOpeningDocumentTag());
        }

        UpgradeWriter.InitializeUpgradesNode(Filename);
        AchievementsIO.InitializeAchievementsNode(Filename);

        using (var streamWriter = new StreamWriter(Filename, true))
        {
            streamWriter.WriteLine(GetClosingDocumentTag());
        }
    }

    private static string GetXmlVersion()
    {
        return "<?xml version='1.0' encoding='utf-8'?>";
    }

    private static string GetOpeningDocumentTag()
    {
        return "<Document>";
    }

    private static string GetClosingDocumentTag()
    {
        return "</Document>";
    }
}
