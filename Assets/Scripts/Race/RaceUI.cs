using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RaceUI : MonoBehaviour
{
    public RaceManager RaceManager;
    public RectTransform CountdownRect;
    public Text Countdown;
    [Range(5, 60)]
    public int UpdateRate;

    Coroutine _lastShowCountdown;

    void Start()
    {
        if(RaceManager == null)
            RaceManager = RaceManager.Instance;
    }

    void Update()
    {
        if (RaceManager.RaceIsOn && RaceManager.CountdownIsOn)
        {
            if (_lastShowCountdown != null)
                StopCoroutine(_lastShowCountdown);
            
            _lastShowCountdown = StartCoroutine(ShowCountdown());
        }
    }

    IEnumerator ShowCountdown()
    {
        var oldUpdateRate = int.MaxValue;
        var updateRateSec = 1f;
        var count = -1;
        CountdownRect.gameObject.SetActive(true);

        while (RaceManager.CountdownIsOn)
        {
            if (count != RaceManager.CurrentCount)
            {
                count = RaceManager.CurrentCount;
                Countdown.text = count.ToString();
            }

            if (oldUpdateRate != UpdateRate)
            {
                oldUpdateRate = UpdateRate;
                updateRateSec = 1f / UpdateRate;
            }
            
            yield return new WaitForSeconds(updateRateSec);
        }

        Countdown.text = "GO!!!";

        yield return new WaitForSeconds(1.5f);

        CountdownRect.gameObject.SetActive(false);
        yield break;
    }
}
