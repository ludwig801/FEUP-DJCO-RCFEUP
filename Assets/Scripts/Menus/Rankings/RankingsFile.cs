using System.IO;
using System.Xml;

public static class RankingsFile {

	public const string Filename = "rankings.sav";

	public static XmlDocument OpenRankigsFile()
	{
		if(!File.Exists(Filename))
			CreateRankingsFile();

		var doc = new XmlDocument();
		doc.Load(Filename);

		return doc;
	}

	public static void CreateRankingsFile()
	{
        using (var writer = new StreamWriter(Filename, false))
        {
            writer.WriteLine(GetXmlVersion());

            writer.WriteLine("<Document>");

            RankingsWriter.InitializeRankings(Filename);

            writer.WriteLine("</Document>");
        }
    }

	private static string GetXmlVersion()
	{
		return "<?xml version='1.0' encoding='utf-8'?>";
	}
}
