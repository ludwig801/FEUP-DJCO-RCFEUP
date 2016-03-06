using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public RectTransform quitDialoguePanel;
	public RectTransform creditsPanel;
	public RectTransform startMenu;

	public Button playButton;
	public Button quitButton;
	public Button creditsButton;

	// Use this for initialization
	void Start () {
		playButton = playButton.GetComponent<Button> ();
		quitButton = quitButton.GetComponent<Button> ();
		creditsButton = creditsButton.GetComponent<Button> ();

		startMenu.gameObject.SetActive(true);
		quitDialoguePanel.gameObject.SetActive(false);
		creditsPanel.gameObject.SetActive (false);
	}
	
	public void PressQuitAtMainMenu(){
		quitDialoguePanel.gameObject.SetActive (true);
		creditsPanel.gameObject.SetActive(false);

		playButton.enabled = false;
		quitButton.enabled = false;
		creditsButton.enabled = false;
	}

	public void PressCreditsAtMainMenu(){
		quitDialoguePanel.gameObject.SetActive(false);
		creditsPanel.gameObject.SetActive(true);

		playButton.enabled = false;
		quitButton.enabled = false;
		creditsButton.enabled = false;
	}

	public void PressBackAtCreditsMenu(){
		quitDialoguePanel.gameObject.SetActive (false);
		creditsPanel.gameObject.SetActive (false);

		playButton.enabled = true;
		quitButton.enabled = true;
		creditsButton.enabled = true;
	}

	public void PressNoAtQuitMenu(){
		quitDialoguePanel.gameObject.SetActive (false);
		creditsPanel.gameObject.SetActive (false);

		playButton.enabled = true;
		quitButton.enabled = true;
		creditsButton.enabled = true;
	}

	public void PressStart(){
		SceneManager.LoadScene ("GameScene");
	}

    public void PressUpgrades()
    {
        SceneManager.LoadScene("UpgradesMenuScene");
    }

	public void PressYesAtQuitMenu(){
		Application.Quit();
	}
}
