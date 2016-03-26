using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenuUI : MonoBehaviour
{
    public RaceManager RaceManager;
    public RectTransform Container;
    public Button Resume, Restart;
    public float AnimSpeed;
    public Utils.WindowPositions HidePosition, ShowPosition;

    [SerializeField]
    bool _visible;
    Coroutine _windowAnim;
    RectTransform _rectTransform;

    void Start()
    {
        if (RaceManager == null)
            RaceManager = RaceManager.Instance;

        _rectTransform = GetComponent<RectTransform>();
        Resume.interactable = false;
        Restart.interactable = false;

        _visible = true;
        SetVisible(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RaceManager.SetPaused(!RaceManager.State.Paused);
            SetVisible(RaceManager.State.Paused);
            Resume.interactable = RaceManager.State.Started;
            Restart.interactable = RaceManager.State.Started && RaceManager.State.Ongoing;
        }

        if (RaceManager.State.Finished)
            SetVisible(false);
    }

    public void SetVisible(bool value)
    {
        if (_visible != value)
        {
            _visible = value;

            if (_windowAnim != null)
                StopCoroutine(_windowAnim);
            
            _windowAnim = StartCoroutine(AnimateWindow());
        }
    }

    IEnumerator AnimateWindow()
    {
        Vector2 targetMin, targetMax;
        if (_visible)
        {
            Utils.GetAnchorsForWindowPosition(ShowPosition, out targetMin, out targetMax);
            while (true)
            {
                if (Utils.LerpRectTransformAnchors(_rectTransform, targetMin, targetMax, Time.unscaledDeltaTime * AnimSpeed))
                    yield break;

                yield return null;
            }
        }
        else
        {
            Utils.GetAnchorsForWindowPosition(HidePosition, out targetMin, out targetMax);
            while (true)
            {
                if (Utils.LerpRectTransformAnchors(_rectTransform, targetMin, targetMax, Time.unscaledDeltaTime * AnimSpeed))
                    yield break;

                yield return null;
            }
        }
    }

    public void OnResumePressed()
    {
        SetVisible(false);
        RaceManager.SetPaused(false);
    }

    public void OnRestartPressed()
    {
        SetVisible(false);
        RaceManager.SetPaused(false);
        RaceManager.NewRace(false);
    }
}
