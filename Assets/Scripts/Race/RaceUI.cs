using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RaceUI : MonoBehaviour
{
    public RectTransform CountdownRect;
    public Text Countdown;

    Coroutine _lastShowCountdown;
    [SerializeField]
    RaceManager _raceManager;

    void Start()
    {
        _raceManager = RaceManager.Instance;
    }

    void Update()
    {
        if (_raceManager.RaceIsOn && _raceManager.CountdownIsOn)
        {
            if (_lastShowCountdown != null)
                StopCoroutine(_lastShowCountdown);
            _lastShowCountdown = StartCoroutine(ShowCountdown());
        }
    }

    IEnumerator ShowCountdown()
    {
        var count = -1;
        CountdownRect.gameObject.SetActive(true);

        while (_raceManager.CountdownIsOn)
        {
            if (count != _raceManager.CurrentCount)
            {
                count = _raceManager.CurrentCount;
                Countdown.text = count.ToString();
            }
            
            yield return null;
        }

        Countdown.text = "GO!!!";

        yield return new WaitForSeconds(1.5f);

        CountdownRect.gameObject.SetActive(false);

        yield break;
    }
}
