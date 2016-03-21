using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BackToMainMenu : MonoBehaviour {

	public void BackToMenu(){
		SceneManager.LoadScene ("MainMenu");
	}
}
