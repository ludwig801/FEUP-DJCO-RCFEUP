using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class RaceOverUI : MonoBehaviour
{
    public RaceManager RaceManager;
    public RectTransform Container;
    public Text Title, TimeText, Description;
    public Button MainMenu, Restart;
    public float AnimSpeed;
    public Utils.WindowPositions HidePosition, ShowPosition;
    [Range(1, 60)]
    public int CheckFinishRate, CheckRaceEndRate;

    [SerializeField]
    bool _visible;
    Coroutine _windowAnim;
    RectTransform _rectTransform;
    Car _car;

    void Start()
    {
        if (RaceManager == null)
            RaceManager = RaceManager.Instance;

        _rectTransform = GetComponent<RectTransform>();
        _car = GetComponentInParent<CarCanvas>().Car;

        _visible = true;
        SetVisible(false);

        StartCoroutine(CheckRaceFinished());
    }

    IEnumerator CheckRaceFinished()
    {
        var oldFinished = true;
        var oldCheckRate = int.MaxValue;
        var refreshRateSec = 1f;

        while (true)
        {
            if (oldFinished != _car.Finished)
            {
                oldFinished = _car.Finished;
                SetVisible(_car.Finished);
            }

            if (oldCheckRate != CheckFinishRate)
            {
                oldCheckRate = CheckFinishRate;
                refreshRateSec = 1f / CheckFinishRate;
            }

            yield return new WaitForSeconds(refreshRateSec);
        }
    }

    public void SetVisible(bool value)
    {
        if (_visible != value)
        {
            _visible = value;

            if (_windowAnim != null)
                StopCoroutine(_windowAnim);

            _windowAnim = StartCoroutine(AnimateWindow());

            if (value)
                StartCoroutine(WaitForRaceEnd());
        }
    }

    public void OnMainMenuPressed()
    {
        MainMenu.interactable = false;
        RaceManager.OnQuitRace(true);
    }

    public void OnRestartPressed()
    {
        Restart.interactable = false;
        RaceManager.NewRace(true);
    }

    IEnumerator WaitForRaceEnd()
    {
        var oldRefreshRate = int.MaxValue;
        var refreshRateSec = 1f;

        TimeText.text = string.Concat("Your time: ", Utils.GetCounterFormattedString(_car.LapTimeCounter.RaceTime));
        Description.text = string.Concat("Waiting for other players to finish...");
        MainMenu.interactable = false;
        Restart.interactable = false;

        while (!RaceManager.State.Finished)
        {
            if (oldRefreshRate != CheckRaceEndRate)
            {
                oldRefreshRate = CheckRaceEndRate;
                refreshRateSec = 1f / CheckRaceEndRate;
            }

            yield return new WaitForSeconds(refreshRateSec);
        }

        MainMenu.interactable = true;
        Restart.interactable = true;

        TimeText.text = string.Concat("Your time: ", Utils.GetCounterFormattedString(_car.LapTimeCounter.RaceTime));

        if(_car.RankingsPlace > 0)
            Description.text = string.Concat("You earned the ", _car.RankingsPlace, Utils.GetOrdinalEnding(_car.RankingsPlace), " place in the rankings!");
        else
            Description.text = string.Concat("Not enough to be on the rankings, though...");
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
}
