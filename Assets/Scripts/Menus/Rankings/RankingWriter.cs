using System.IO;
using System.Xml;
using System.Collections.Generic;

public static class RankingsWriter
{
	private static string FormatRankingNode(int place, string playerName, float time)
	{
		return string.Concat("\t<Ranking place='", place, "' playerName='", playerName, "' time='", time.ToString(), "'/>\n");
	}

    private static string FomatRankingNode(Ranking rankingObj)
    {
        return string.Concat("\t<Ranking place='", rankingObj.Place, "' playerName='", rankingObj.PlayerName, "' time='", rankingObj.PlayerTime.ToString(), "'/>\n");
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

    public static void WriteToFile(List<Ranking> list = null)
    {
        using (var writer = new StreamWriter(RankingsReader.Filename, false))
        {
            writer.WriteLine("<Document>");
            writer.WriteLine("    <Rankings>");

            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    writer.Write(FomatRankingNode(list[i]));
                }
            }

            writer.WriteLine("    </Rankings>");
            writer.WriteLine("</Document>");

            writer.Close();
        }
    }
}
