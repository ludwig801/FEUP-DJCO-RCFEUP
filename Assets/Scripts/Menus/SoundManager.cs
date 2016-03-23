using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour {

	public AudioSource musicSource;
	public AudioSource efxSource;
	public static SoundManager instance = null;                  

	void Start ()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
			
		DontDestroyOnLoad (gameObject);
	}

	public void PlaySingle(AudioClip clip)
	{
		efxSource.clip = clip;
		efxSource.Play ();
	}
}
