using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public GameObject quitDialoguePanel;
	public GameObject creditsPanel;
	public GameObject startMenu;

	public Button playButton;
	public Button quitButton;
	public Button creditsButton;

	// Use this for initialization
	void Start () {
		playButton = playButton.GetComponent<Button> ();
		quitButton = quitButton.GetComponent<Button> ();
		creditsButton = creditsButton.GetComponent<Button> ();

		startMenu.SetActive(true);
		quitDialoguePanel.SetActive(false);
		creditsPanel.SetActive (false);
	}
	
	public void PressQuitAtMainMenu(){
		quitDialoguePanel.SetActive (true);
		creditsPanel.SetActive(false);

		playButton.enabled = false;
		quitButton.enabled = false;
		creditsButton.enabled = false;
	}

	public void PressCreditsAtMainMenu(){
		quitDialoguePanel.SetActive(false);
		creditsPanel.SetActive(true);

		playButton.enabled = false;
		quitButton.enabled = false;
		creditsButton.enabled = false;
	}

	public void PressBackAtCreditsMenu(){
		quitDialoguePanel.SetActive (false);
		creditsPanel.SetActive (false);

		playButton.enabled = true;
		quitButton.enabled = true;
		creditsButton.enabled = true;
	}

	public void PressNoAtQuitMenu(){
		quitDialoguePanel.SetActive (false);
		creditsPanel.SetActive (false);

		playButton.enabled = true;
		quitButton.enabled = true;
		creditsButton.enabled = true;
	}

	public void PressStart(){
		SceneManager.LoadScene ("MainMenu");
	}

	public void PressYesAtQuitMenu(){
		Application.Quit();
	}
}
