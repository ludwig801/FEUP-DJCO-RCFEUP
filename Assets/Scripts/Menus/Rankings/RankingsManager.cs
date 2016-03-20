using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RankingsManager : MonoBehaviour
{
    public int MaxNameLength;
    public RankingUI[] Rankings;


    void Start()
    {
        SetRankings();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void SetRankings()
    {
        for (int i = 0; i < 10; i++)
        {
            SetRankingsPlace(i);
        }
    }

    private void SetRankingsPlace(int place)
    {
        var rank = RankingWriter.GetRanking(place + 1);
        var ranking = Rankings[place];

        var name = rank.Attributes[1].Value;
        ranking.RacerName.text = name.Substring(0, Mathf.Min(MaxNameLength, name.Length));
        ranking.RaceTime.text = Utils.GetCounterFormattedString(float.Parse(rank.Attributes[2].Value));
    }
}
