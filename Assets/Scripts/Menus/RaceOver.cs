using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RaceOver : MonoBehaviour {

	public InputField input;
	private InputField.SubmitEvent se;
	public Text playerMessage;
	public Text raceTime;

	private int playerPosition;
	private float playerTime;

	void Start(){
		input = input.GetComponent<InputField> ();
		se = new InputField.SubmitEvent ();
		se.AddListener (SubmitPlayerName);
		input.onEndEdit = se;
		//GET SINGLETON TIME INFORMATION
		playerPosition = RankingWriter.GetPlayerPosition(250f);
		playerTime = 250f;
		if (playerPosition > 10) {
			playerMessage.text = "YOU DIDN'T GET INTO THE TOP TEN";
		} else {
			playerMessage.text = "CONGRATULATIONS! YOU ARE IN " + GetPositionString (playerPosition) + " PLACE!"; 
		}
	}

	private void SubmitPlayerName(string arg0)
	{
		string playername = input.text.Trim();
		Debug.Log(playername.ToUpper());
		RankingWriter.UpdateRankings (playerPosition, playername, playerTime);
	}

	private string GetPositionString(int position){
		switch (position)
		{
		case 1:
			return "1ST";
			break;
		case 2:
			return "2ND";
			break;
		case 3:
			return "3RD";
			break;
		default:
			return position.ToString() + "TH";
			break;
		}
	}
}
