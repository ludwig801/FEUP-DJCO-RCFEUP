using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RaceUI : MonoBehaviour
{
    public RaceManager RaceManager;
    public RectTransform CountdownRect;
    public Text Countdown;
    public AudioSource CountdownAudio;
    [Range(5, 60)]
    public int UpdateRate;

    void Start()
    {
        if(RaceManager == null)
            RaceManager = RaceManager.Instance;

        StartCoroutine(ShowCountdown());
    }

    IEnumerator ShowCountdown()
    {
        var oldUpdateRate = int.MaxValue;
        var updateRateSec = 1f;
        var count = -1;
        var goBip = false;
        var active = false;

        while (true)
        {
            if (RaceManager.Countdown.Running)
            {
                if (goBip)
                    goBip = false;

                if (!active)
                {
                    active = true;
                    CountdownRect.gameObject.SetActive(active);
                }

                CountdownAudio.pitch = 1;

                if (count != RaceManager.Countdown.CurrentCount)
                {
                    count = RaceManager.Countdown.CurrentCount;
                    Countdown.text = count.ToString();
                    CountdownAudio.Play();
                }
            }
            else if (RaceManager.State.Ongoing && !goBip)
            {
                CountdownAudio.pitch = 1.5f;
                CountdownAudio.Play();
                Countdown.text = "GO!!!";

                goBip = true;
                active = false;
                CountdownRect.gameObject.SetActive(active);
            }

            if (oldUpdateRate != UpdateRate)
            {
                oldUpdateRate = UpdateRate;
                updateRateSec = 1f / UpdateRate;
            }
            
            yield return new WaitForSeconds(updateRateSec);
        }
    }
}
