using System;
using System.IO;
using System.Xml;
using UnityEngine;

public static class RankingWriter
{
	public static void InitializeRankings(string filename)
	{
		using (var writer = new StreamWriter (filename, true))
		{
			writer.WriteLine (OpeningParentNode ());

			for (int i = 1; i <= 10; i++) 
			{
				writer.Write (ChildNode (i, "Player", (float)5940.99));
			}

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

	private static string ChildNode(int place, string name, float time)
	{
		return "\t<Ranking place='" + place + "' name='" + name + "' time='" + time.ToString() + "'/>\n";
	}

	public static void ReplaceRanking(int place, string name, float time)
	{
		var xmlDoc = RankingsFile.OpenRankigsFile ();

		var node = GetRankingsNode (xmlDoc, place);

		node.Attributes [1].Value = name;
		node.Attributes [2].Value = time.ToString ();
	}

	public static XmlNode GetRankingsNode(XmlDocument xmlDoc, int place)
	{
		var elements = xmlDoc.SelectSingleNode ("/Rankings").ChildNodes;

		foreach (XmlNode element in elements)
		{
			if (int.Parse (element.Attributes [0].Value) == place)
			{
				return element;
			}
		}

		return null;
	}

	public static XmlNode GetRanking(int place)
	{
		var xmlDoc = RankingsFile.OpenRankigsFile ();

		return GetRankingsNode (xmlDoc, place);
	}
}
