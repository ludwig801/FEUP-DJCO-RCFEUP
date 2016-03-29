using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class PanelFadeInOut : MonoBehaviour
{
    public float FadeInTime;
    public float IdleTime;
    public float FadeOutTime;
    public bool Finished;

    CanvasGroup _canvasGroup;

    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Finished = false;

        StartCoroutine(FadeInOut(FadeInTime, IdleTime, FadeOutTime));
    }

    IEnumerator FadeInOut(float inTime, float idleTime, float outTime)
    {
        var inTimeInverted = 1f / inTime;
        var outTimeInverted = 1f / outTime;
        var timePassed = 0f;

        var scene = SceneManager.GetActiveScene();
        while (!scene.isLoaded)
        {
            yield return null;
        }

        _canvasGroup.alpha = 0;
        while (timePassed < inTime)
        {
            _canvasGroup.alpha = timePassed * inTimeInverted;
            timePassed += Time.unscaledDeltaTime;
            yield return null;
        }

        _canvasGroup.alpha = 1;
        timePassed = 0f;
        while (timePassed < idleTime)
        {
            timePassed += Time.unscaledDeltaTime;
            yield return null;
        }

        timePassed = 0f;
        while (timePassed < outTime)
        {
            _canvasGroup.alpha = 1 - timePassed * outTimeInverted;
            timePassed += Time.unscaledDeltaTime;
            yield return null;
        }

        _canvasGroup.alpha = 0f;

        Finished = true;
    }
}