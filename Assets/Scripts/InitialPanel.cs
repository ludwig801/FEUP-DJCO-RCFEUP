using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialPanel : MonoBehaviour
{
    public PanelFadeInOut FadeInOut;

    AsyncOperation _loadMainMenu;

    void Start()
    {
        _loadMainMenu = SceneManager.LoadSceneAsync("MainMenu");
        _loadMainMenu.allowSceneActivation = false;
    }

    void Update()
    {
        if (FadeInOut.Finished && _loadMainMenu.progress > 0.85f)
            _loadMainMenu.allowSceneActivation = true;
    }
}
