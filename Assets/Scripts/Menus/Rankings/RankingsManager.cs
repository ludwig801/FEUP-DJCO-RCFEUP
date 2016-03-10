using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RankingsManager : MonoBehaviour {

	public RectTransform[] rankings;

	void Start()
	{
		SetRankings ();
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
		for (int i = 1; i <= 10; i++)
		{
			SetRankingsPlace (i);
		}
	}

	private void SetRankingsPlace(int place)
	{
		var rank = RankingWriter.GetRanking (place);
		var info = rankings [place - 1].gameObject.GetComponentsInChildren<Text> ();

		info [1].text = rank.Attributes [1].Value;
		float time = float.Parse(rank.Attributes [2].Value);
		info [2].text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.Floor(time/60), Mathf.Floor(time%60), (int)(((decimal)time%1)*100));
	}
}
