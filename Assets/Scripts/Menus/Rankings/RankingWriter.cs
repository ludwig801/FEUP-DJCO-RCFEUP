using System.IO;
using System.Xml;
using System.Collections.Generic;

public static class RankingsWriter
{
	public static void InitializeRankings(string filename)
	{
		using (var writer = new StreamWriter (filename, true))
		{
			writer.WriteLine (OpeningParentNode ());
			writer.WriteLine (OlosingParentNode ());
		}
	}

	private static string OpeningParentNode()
	{
		return "<Rankings>";
	}

	private static string OlosingParentNode()
	{
		return "</Rankings>";
	}

	private static string ChildNode(int place, string playerName, float time)
	{
		return string.Concat("\t<Ranking place='", place, "' playerName='", playerName, "' time='", time.ToString(), "'/>\n");
	}

    private static string ChildNode(Ranking rankingObj)
    {
        return string.Concat("\t<Ranking place='", rankingObj.Place, "' playerName='", rankingObj.PlayerName, "' time='", rankingObj.PlayerTime.ToString(), "'/>\n");
    }

    public static int GetPlayerPosition(float playerTime)
	{
		var xmlDoc = RankingsFile.OpenRankigsFile ();

		var currentRankings = xmlDoc.SelectSingleNode ("/Document/Rankings").ChildNodes;

		int currentPosition = 10;

		foreach (XmlNode rank in currentRankings)
		{
			if (currentPosition < 0)
				break;
			float rankTime = float.Parse (currentRankings[currentPosition-1].Attributes[2].Value);
			if (playerTime < rankTime)
			{
				currentPosition -= 1;
			}
		}

		int playerPosition = currentPosition + 1;

		return playerPosition;
	}

	public static void UpdateRankings(int playerPosition, string playerName, float playerTime)
	{
		var xmlDoc = RankingsFile.OpenRankigsFile ();

		for(int i = 10; i > playerPosition; i--)
		{
			var node = GetRankingsNode (xmlDoc, i-1);
			ReplaceRanking(xmlDoc, i, node.Attributes[1].Value, float.Parse(node.Attributes[2].Value));
		}

		ReplaceRanking(xmlDoc, playerPosition, playerName, playerTime);

		xmlDoc.Save (RankingsFile.Filename);
	}

	private static void ReplaceRanking(XmlDocument xmlDoc, int place, string name, float time)
	{
		var node = GetRankingsNode (xmlDoc, place);

        node.Attributes [1].Value = name;
		node.Attributes [2].Value = time.ToString ();
	}

	public static XmlNode GetRanking(int place)
	{
		var xmlDoc = RankingsFile.OpenRankigsFile ();

		return GetRankingsNode (xmlDoc, place);
	}

	private static XmlNode GetRankingsNode(XmlDocument xmlDoc, int place)
	{
		var elements = xmlDoc.SelectSingleNode ("/Document/Rankings").ChildNodes;

		foreach (XmlNode element in elements)
		{
			if (int.Parse (element.Attributes [0].Value) == place)
			{
				return element;
			}
		}

		return null;
	}

    public static void WriteToFile(List<Ranking> list)
    {
        using (var writer = new StreamWriter(RankingsFile.Filename, false))
        {
            writer.WriteLine("<Document>");
            writer.WriteLine(OpeningParentNode());

            for (int i = 0; i < list.Count; i++)
            {
                writer.Write(ChildNode(list[i]));
            }

            writer.WriteLine(OlosingParentNode());
            writer.WriteLine("</Document>");
        }
    }
}
