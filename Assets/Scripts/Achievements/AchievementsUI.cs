using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class AchievementsUI : MonoBehaviour {

    public Text[] Texts = new Text[3];
    public Toggle[] Checkboxes = new Toggle[3];

	void Start () {

        var totalAchievements = AchievementsInfo.GetTotalAchievementsCount();

        for(var i = 0; i < Texts.Length; i++)
        {
            Texts.ElementAt(i).text = AchievementsInfo.GetAchievementDescription(i);
            Checkboxes.ElementAt(i).isOn = AchievementsIO.GetAchievementStatus(i) == 1;
        }

	}

    public void OnBackPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }

}