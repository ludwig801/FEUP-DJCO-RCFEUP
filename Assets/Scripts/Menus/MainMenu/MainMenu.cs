using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public ShowHidePanel QuitGame, Credits, Main, Rankings, PlayMenu;
    ShowHidePanel _currentPanel;

    void Start()
    {
        ChangeCurrentPanelTo(Main);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            OnBackPressed();
    }

    public void OnCreditsPressed()
    {
        ChangeCurrentPanelTo(Credits);
    }

    public void OnBackPressed()
    {
        if (_currentPanel == Main)
            OnQuitPressed();
        else
            ChangeCurrentPanelTo(Main);
    }

    public void OnQuitPressed()
    {
        ChangeCurrentPanelTo(QuitGame);
    }

    public void OnQuitCancelled()
    {
        ChangeCurrentPanelTo(Main);
    }

    public void OnQuitConfirmed()
    {
        Application.Quit();
    }

    public void OnPlayPressed()
    {
        ChangeCurrentPanelTo(PlayMenu);
    }

    public void OnUpgradesPressed()
    {
        SceneManager.LoadScene("UpgradesMenuScene");
    }

    public void OnRankingsPressed()
    {
        ChangeCurrentPanelTo(Rankings);
    }

    public void OnAchievementsPressed()
    {
        SceneManager.LoadScene("AchievementsScene");
    }

    private void ChangeCurrentPanelTo(ShowHidePanel newPanel)
    {
        Main.Visible = (newPanel == Main);
        QuitGame.Visible = (newPanel == QuitGame);
        Credits.Visible = (newPanel == Credits);
        Rankings.Visible = (newPanel == Rankings);
        PlayMenu.Visible = (newPanel == PlayMenu);

        _currentPanel = newPanel;
    }
}
