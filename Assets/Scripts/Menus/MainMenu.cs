using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public Canvas quitMenu;
	public Canvas creditsMenu;
	public Canvas mainMenu;

	public Button playButton;
	public Button quitButton;
	public Button creditsButton;

	// Use this for initialization
	void Start () {
		quitMenu = quitMenu.GetComponent<Canvas> ();
		creditsMenu = creditsMenu.GetComponent<Canvas> ();
		mainMenu = mainMenu.GetComponent<Canvas> ();

		playButton = playButton.GetComponent<Button> ();
		quitButton = quitButton.GetComponent<Button> ();
		creditsButton = creditsButton.GetComponent<Button> ();

		quitMenu.enabled = false;
		creditsMenu.enabled = false;
	}
	
	public void PressQuitAtMainMenu(){
		quitMenu.enabled = true;
		creditsMenu.enabled = false;

		playButton.enabled = false;
		quitButton.enabled = false;
		creditsButton.enabled = false;
	}

	public void PressCreditsAtMainMenu(){
		quitMenu.enabled = false;
		creditsMenu.enabled = true;

		playButton.enabled = false;
		quitButton.enabled = false;
		creditsButton.enabled = false;
	}

	public void PressBackAtCreditsMenu(){
		quitMenu.enabled = false;
		creditsMenu.enabled = false;

		playButton.enabled = true;
		quitButton.enabled = true;
		creditsButton.enabled = true;
	}

	public void PressNoAtQuitMenu(){
		quitMenu.enabled = false;
		creditsMenu.enabled = false;

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
