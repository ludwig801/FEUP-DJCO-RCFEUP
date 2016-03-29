using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class NewAchievementUI : MonoBehaviour
{
    public float LerpSpeed;
    public float ShowTime;
    public Text Description;

    private CanvasGroup _canvasGroup;
    private Coroutine _lastShow;

    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ShowAchievement(string name)
    {
        Description.text = name;

        if (_lastShow != null)
            StopCoroutine(_lastShow);

        _lastShow = StartCoroutine(Show());
    }

    IEnumerator Show()
    {
        var timeLeft = ShowTime;
        var threshold = 0.25f * ShowTime;
        var invertedThreshold = 1f / threshold;
        _canvasGroup.alpha = 1;

        while (timeLeft > threshold)
        {
            timeLeft -= Time.unscaledDeltaTime;

            yield return null;
        }

        while (timeLeft > 0)
        {
            timeLeft -= Time.unscaledDeltaTime * LerpSpeed;

            _canvasGroup.alpha = invertedThreshold * timeLeft;

            yield return null;
        }

        _canvasGroup.alpha = 0;
    }
}
