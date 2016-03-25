using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class RankingsManager : MonoBehaviour
{
    public int MaxNameLength;
    public List<RankingUI> RankingsUI;

    void Start()
    {
        LoadAllRankings();
    }

    private void LoadAllRankings()
    {
        var rankings = RankingsReader.GetAllRankings();

        for (int i = 0; i < RankingsUI.Count; i++)
        {
            var rankingUI = RankingsUI[i];
            var ranking = GetRanking(rankings, (i+1));

            if (ranking != null)
            {
                rankingUI.RacerName.text = ranking.PlayerName.Substring(0, Mathf.Min(MaxNameLength, ranking.PlayerName.Length));
                rankingUI.RaceTime.text = Utils.GetCounterFormattedString(ranking.PlayerTime);
            }
            else
            {
                rankingUI.RacerName.text = string.Empty;
                rankingUI.RaceTime.text = string.Empty;
            }
        }
    }

    private Ranking GetRanking(List<Ranking> list, int place)
    {
        foreach (var ranking in list)
        {
            if (ranking.Place == place)
                return ranking;
        }

        return null;
    }

    public void ResetAllRankings()
    {
        var empty = new List<Ranking>();
        RankingsWriter.WriteToFile(empty);
        LoadAllRankings();
    }
}
