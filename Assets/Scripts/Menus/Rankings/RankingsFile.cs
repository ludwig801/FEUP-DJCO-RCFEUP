using System.IO;
using System.Xml;

public static class RankingsFile {

	public const string Filename = "rankings.sav";

	public static XmlDocument OpenRankigsFile()
	{
		if(!File.Exists(Filename))
		{
			CreateRankingsFile();
		}

		var doc = new XmlDocument();
		doc.Load(Filename);

		return doc;
	}

	private static void CreateRankingsFile()
	{
		using (var writer = new StreamWriter(Filename, true))
		{
			writer.WriteLine(GetXmlVersion());
		}

		RankingWriter.InitializeRankings(Filename);
	}

	private static string GetXmlVersion()
	{
		return "<?xml version='1.0' encoding='utf-8'?>";
	}
}
