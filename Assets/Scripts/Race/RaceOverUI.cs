using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class RaceOverUI : MonoBehaviour
{
    public RaceManager RaceManager;
    public RectTransform Container;
    public Text TimeText, Description;
    public float AnimSpeed;
    public Utils.WindowPositions HidePosition, ShowPosition;
    [Range(1, 60)]
    public int CheckFinishRate;

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
        var oldCheckRate = int.MaxValue;
        var refreshRateSec = 1f;

        while (true)
        {
            if (oldCheckRate != CheckFinishRate)
            {
                oldCheckRate = CheckFinishRate;
                refreshRateSec = 1f / CheckFinishRate;
            }

            SetVisible(RaceManager.State.Finished);

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
                OnRaceFinish();
        }
    }

    void OnRaceFinish()
    {
        TimeText.text = string.Concat("Your time: ", Utils.GetCounterFormattedString(_car.LapTimeCounter.TotalTime));

        if (RaceManager.State.WinnerRanking != null)
            Description.text = string.Concat("You earned the ", RaceManager.State.WinnerRanking.Place, Utils.GetOrdinalEnding(RaceManager.State.WinnerRanking.Place), " place in the rankings!");
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

    public void OnQuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
