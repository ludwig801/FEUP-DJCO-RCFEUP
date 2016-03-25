using System.Xml;
using System.Collections.Generic;
using UnityEngine;

public static class RankingsReader
{
    public static List<Ranking> GetAllRankings()
    {
        var list = new List<Ranking>();

        var rankingsFile = RankingsFile.OpenRankigsFile();
        var currentRankings = rankingsFile.SelectSingleNode("/Document/Rankings").ChildNodes;

        foreach (XmlNode xmlRanking in currentRankings)
        {
            list.Add(ConvertToRanking(xmlRanking));
        }

        list.Sort();

        return list;
    }

    public static Ranking ConvertToRanking(XmlNode node)
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
