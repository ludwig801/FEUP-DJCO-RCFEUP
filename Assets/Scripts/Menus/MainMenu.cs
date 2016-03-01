using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public Canvas quitMenu;
	public Button playButton;
	public Button quitButton;

	// Use this for initialization
	void Start () {
		quitMenu = quitMenu.GetComponent<Canvas> ();
		playButton = playButton.GetComponent<Button> ();
		quitButton = quitButton.GetComponent<Button> ();
		quitMenu.enabled = false;
	}
	
	public void ExitPress(){
		quitMenu.enabled = true;
		playButton.enabled = false;
		quitButton.enabled = false;
	}

	public void NoPress(){
		quitMenu.enabled = false;
		playButton.enabled = true;
		quitButton.enabled = true;
	}

	public void StartLevel(){
		SceneManager.LoadScene ("MainMenu");
	}

	public void QuitGame(){
		Application.Quit();
	}
}
