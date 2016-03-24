using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour {

	public AudioSource musicSource;
	public AudioSource efxSource;

	public void PlaySingle(AudioClip clip)
	{
		efxSource.clip = clip;
		efxSource.Play ();
	}
}
