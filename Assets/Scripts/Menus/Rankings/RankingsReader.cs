using System.Xml;
using System.Collections.Generic;
using System.IO;

public static class RankingsReader
{
    public const string Filename = "Rankings.sav";

    public static XmlDocument OpenRankigsFile()
    {
        if (!File.Exists(Filename))
            RankingsWriter.WriteToFile();

        var doc = new XmlDocument();
        doc.Load(Filename);

        return doc;
    }

    public static List<Ranking> GetAllRankings()
    {
        var list = new List<Ranking>();

        var rankingsFile = OpenRankigsFile();
        var currentRankings = rankingsFile.SelectSingleNode("/Document/Rankings").ChildNodes;

        foreach (XmlNode xmlRanking in currentRankings)
        {
            list.Add(ParseRanking(xmlRanking));
        }

        list.Sort();

        return list;
    }

    public static Ranking ParseRanking(XmlNode node)
    {
        var newRanking = new Ranking();

        foreach (XmlAttribute attr in node.Attributes)
        {
            switch (attr.Name)
            {
                case "place":
                    newRanking.Place = int.Parse(attr.Value);
                    break;

                case "playerName":
                    newRanking.PlayerName = attr.Value;
                    break;

                case "time":
                    newRanking.PlayerTime = float.Parse(attr.Value);
                    break;
            }
        }

        return newRanking;
    }
}
