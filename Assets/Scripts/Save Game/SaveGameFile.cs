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
        }

        UpgradeWriter.InitializeUpgradesNode(Filename);
    }

    private static string GetXmlVersion()
    {
        return "<?xml version='1.0' encoding='utf-8'?>";
    }
}
