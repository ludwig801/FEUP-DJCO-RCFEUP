using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public RectTransform Container;

    public Button ResumeBtn, RestartBtn, QuitRaceBtn;

    public bool IsPaused;

    public bool Pause
    {
        get;
        set;
    }

    void Start()
    {
        ResumeBtn = ResumeBtn.GetComponent<Button>();
        QuitRaceBtn = QuitRaceBtn.GetComponent<Button>();
        RestartBtn = RestartBtn.GetComponent<Button>();

        ResumeBtn.interactable = false;

        Pause = true;
        IsPaused = !Pause;

        StartCoroutine(UpdateMenu());
    }

    IEnumerator UpdateMenu()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause = !Pause;
            }

            if (Pause != IsPaused)
            {
                IsPaused = Pause;

                Container.gameObject.SetActive(IsPaused);
                Time.timeScale = IsPaused ? 0 : 1;

                if (!ResumeBtn.interactable && !IsPaused)
                    ResumeBtn.interactable = true;
            }

            yield return null;
        }
    }

    public void OnQuitRace()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
