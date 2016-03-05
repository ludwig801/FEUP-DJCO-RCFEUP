using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

	public RectTransform pauseMenu;

	public Button continueButton;
	public Button quitButton;
	public Button restartButton;

	private bool isPaused;

	// Use this for initialization
	void Start () {
		continueButton = continueButton.GetComponent<Button> ();
		quitButton = quitButton.GetComponent<Button> ();
		restartButton = restartButton.GetComponent<Button> ();

		pauseMenu.gameObject.SetActive (false);
		isPaused = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (isPaused) {
				isPaused = false;
				Time.timeScale = 1;
				pauseMenu.gameObject.SetActive (false);
			} else {
				isPaused = true;
				Time.timeScale = 0;
				pauseMenu.gameObject.SetActive (true);
			}
		}
	}

	public void pressContinue(){
		if (isPaused) {
			isPaused = false;
			Time.timeScale = 1;
			pauseMenu.gameObject.SetActive (false);
		}
	}

	public void pressQuit(){
		Time.timeScale = 1;
		SceneManager.LoadScene ("MainMenu");
	}

	public void pressRestart(){
		Time.timeScale = 1;
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}
}
