using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RaceOverUI : MonoBehaviour
{
    public RaceManager RaceManager;
    public RectTransform Container;
    public float AnimSpeed;
    public Utils.WindowPositions HidePosition, ShowPosition;
    [Range(1, 60)]
    public int CheckFinishRate;

    [SerializeField]
    bool _visible;
    Coroutine _windowAnim;
    RectTransform _rectTransform;

    void Start()
    {
        if (RaceManager == null)
            RaceManager = RaceManager.Instance;

        _rectTransform = GetComponent<RectTransform>();

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

    public void OnQuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
