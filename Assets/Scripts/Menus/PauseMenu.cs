using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public RectTransform Background;

    public Button ResumeBtn;
    public Button RestartBtn;
    public Button QuitRaceBtn;

    [SerializeField]
    private bool _isPaused;

    void Start()
    {
        ResumeBtn = ResumeBtn.GetComponent<Button>();
        QuitRaceBtn = QuitRaceBtn.GetComponent<Button>();
        RestartBtn = RestartBtn.GetComponent<Button>();

        Background.gameObject.SetActive(false);
        _isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
                OnResume();
            else
                OnPause();
        }
    }

    public void OnPause()
    {
        _isPaused = true;
        Time.timeScale = 0;
        Background.gameObject.SetActive(true);
    }

    public void OnResume()
    {
        _isPaused = false;
        Time.timeScale = 1;
        Background.gameObject.SetActive(false);
    }

    public void OnQuitRace()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnRestart()
    {
        Time.timeScale = 1;
        _isPaused = false;
    }
}
