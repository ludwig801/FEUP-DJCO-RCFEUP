using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RaceOver : MonoBehaviour {

	public Text inputField;
	public Text playerMessage;
	public Text raceTime;

	private int playerPosition;
	void Start(){
		//GET SINGLETON TIME INFORMATION
		playerPosition = RankingWriter.GetPlayerPosition(4000f); // time goes here
		if (playerPosition > 10) {
			playerMessage.text = "YOU DIDN'T GET INTO THE TOP TEN";
		} else {
			playerMessage.text = "CONGRATULATIONS! YOU ARE IN " + GetPositionString (playerPosition) + " PLACE!"; 
		}
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Return))
		{
			string playerName = inputField.text.Trim ();
			if (playerName != "")
			{
				
			}
		}
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
