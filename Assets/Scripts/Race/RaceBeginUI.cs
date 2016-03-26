using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RaceBeginUI : MonoBehaviour
{
    public RaceManager RaceManager;
    public RectTransform Container;
    public Button StartRace;
    public InputField NameInput;
    public float AnimSpeed;
    public Utils.WindowPositions HidePosition, ShowPosition;
    public bool Visible;

    Coroutine _windowAnim;
    RectTransform _rectTransform;
    Car _car;

    void Start()
    {
        if (RaceManager == null)
            RaceManager = RaceManager.Instance;

        _rectTransform = GetComponent<RectTransform>();
        _car = GetComponentInParent<CarCanvas>().Car;

        StartRace.interactable = false;
        SetVisible(true);
    }

    public void SetVisible(bool value)
    {
        if (Visible != value)
        {
            Visible = value;

            if (_windowAnim != null)
                StopCoroutine(_windowAnim);

            _windowAnim = StartCoroutine(AnimateWindow());
        }
    }

    IEnumerator AnimateWindow()
    {
        Vector2 targetMin, targetMax;
        if (Visible)
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

    public void OnInputEdited(string newVal)
    {
        if (newVal.Length > 0)
        {
            NameInput.text = NameInput.text.ToUpper();
            StartRace.interactable = true;
        }
        else
        {
            StartRace.interactable = false;
        }
    }

    public void OnStartRace()
    {
        if (StartRace.interactable)
        {
            _car.PlayerName = NameInput.text;
            RaceManager.NewRace(true);
            StartRace.interactable = false;
            NameInput.interactable = false;
            StartCoroutine(WaitForRaceBegin());
        }
    }

    IEnumerator WaitForRaceBegin()
    {
        while (!RaceManager.Countdown.Running && !RaceManager.State.Ongoing)
            yield return null;

        SetVisible(false);
    }
}
