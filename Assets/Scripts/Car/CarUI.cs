using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class CarUI : MonoBehaviour
{
    public RaceManager RaceManager;
    public Car Car;
    public RectTransform LapPartialPrefab;
    public Text Checkpoint, Lap, LapTime;
    public RectTransform LapPartialsRect, BestPartialsRect;
    public Color BestPartialColor, WorstPartialColor;
    public Image PowerUp, PowerUpFill;
    public Text PowerUpText, CoinCount;
    public bool RunLapStats, RunTimeStats, RunCollectablesStats;
    [Range(1, 60)]
    public int LapRefreshRate, TimesRefreshRate, CoinsRefreshRate, PowerUpRefreshRate;

    private List<Text> _lapPartials, _bestPartials;

    void Start()
    {
        RaceManager = RaceManager.Instance;
        _lapPartials = new List<Text>();
        _bestPartials = new List<Text>();

        StartCoroutine(ShowLapStats());
        StartCoroutine(ShowTimeStats());
        StartCoroutine(ShowCoins());
        StartCoroutine(ShowPowerUp());
    }

    void Update()
    {
        if (RaceManager.State.Finished)
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
        }
    }

    IEnumerator ShowLapStats()
    {
        var oldLap = -1;
        var oldCheckpoint = -1;
        var lapCounter = Car.LapCounter;
        var oldRefreshRate = int.MaxValue;
        var refreshRateSec = 1f;

        if (Lap == null || Checkpoint == null)
        {
            Debug.LogWarning(string.Concat(Lap.name, " or ", Checkpoint.name, " is null. Coroutine will not proceed and UI values will not be updated."));
            yield break;
        }

        while (true)
        {
            if (RunLapStats)
            {
                if (oldLap != lapCounter.CurrentLap)
                {
                    oldLap = lapCounter.CurrentLap;
                    Lap.text = string.Concat("Lap: ", lapCounter.CurrentLapPlusOne);
                }

                if (oldCheckpoint != lapCounter.CurrentCheckpoint)
                {
                    oldCheckpoint = lapCounter.CurrentCheckpoint;
                    Checkpoint.text = string.Concat("Checkpoint: ", oldCheckpoint);
                }
            }

            if (oldRefreshRate != LapRefreshRate)
            {
                oldRefreshRate = LapRefreshRate;
                refreshRateSec = 1f / LapRefreshRate;
            }

            yield return new WaitForSeconds(refreshRateSec);
        }
    }

    IEnumerator ShowTimeStats()
    {
        var oldCheckpoint = -1;
        var lapCounter = Car.LapCounter;
        var lapTimeCounter = Car.LapTimeCounter;
        var oldPartialsCount = 0;
        var oldRefreshRate = int.MaxValue;
        var refreshRateSec = 1f;

        while (true)
        {
            if (RunTimeStats && RaceManager.State.Ongoing && lapTimeCounter.CurrentLapPartials != null)
            {
                if (oldCheckpoint != lapCounter.CurrentCheckpoint)
                {
                    oldCheckpoint = lapCounter.CurrentCheckpoint;
                    var partialsCount = lapTimeCounter.CurrentLapPartials.Count;

                    if (oldPartialsCount > partialsCount)
                    {
                        for (var i = 0; i < _lapPartials.Count; i++)
                        {
                            _lapPartials[i].enabled = false;
                            //_bestPartials[i].enabled = false;
                        }
                    }
                    else if (partialsCount > 0)
                    {
                        var currentPartials = lapTimeCounter.CurrentLapPartials;

                        if (_lapPartials.Count < partialsCount)
                        {
                            var partial = Instantiate(LapPartialPrefab).GetComponent<Text>();
                            partial.transform.SetParent(LapPartialsRect);
                            partial.name = string.Concat("Partial ", partialsCount);
                            _lapPartials.Add(partial);

                            partial = Instantiate(LapPartialPrefab).GetComponent<Text>();
                            partial.transform.SetParent(BestPartialsRect);
                            partial.name = string.Concat("Best ", partialsCount);
                            partial.enabled = true;
                            _bestPartials.Add(partial);
                        }

                        var checkpoint = partialsCount - 1;
                        var currentPartialText = _lapPartials[checkpoint];
                        var currentPartialValue = currentPartials[checkpoint];
                        currentPartialText.text = Utils.GetCounterFormattedString(currentPartialValue);
                        currentPartialText.enabled = true;
                        if (lapTimeCounter.IsBestPartial(checkpoint, currentPartialValue))
                        {
                            _bestPartials[checkpoint].text = currentPartialText.text;
                            _bestPartials[checkpoint].color = BestPartialColor;
                            currentPartialText.color = BestPartialColor;
                        }
                        else
                        {
                            currentPartialText.color = WorstPartialColor;
                        }
                    }
                }

                LapTime.text = string.Concat("Time: ", Utils.GetCounterFormattedString(lapTimeCounter.CurrentLapTime));
            }

            if (oldRefreshRate != TimesRefreshRate)
            {
                oldRefreshRate = TimesRefreshRate;
                refreshRateSec = 1f / TimesRefreshRate;
            }

            yield return new WaitForSeconds(refreshRateSec);
        }
    }

    IEnumerator ShowCoins()
    {
        var oldCoinCount = int.MinValue;
        var lapCounter = Car.LapCounter;
        var lapTimeCounter = Car.LapTimeCounter;
        var oldRefreshRate = int.MaxValue;
        var refreshRateSec = 1f;

        while (true)
        {
            if (RunCollectablesStats)
            {
                if (oldCoinCount != Car.Coins)
                {
                    oldCoinCount = Car.Coins;
                    CoinCount.text = string.Concat(Car.Coins);
                }
            }

            if (oldRefreshRate != CoinsRefreshRate)
            {
                oldRefreshRate = CoinsRefreshRate;
                refreshRateSec = 1f / CoinsRefreshRate;
            }

            yield return new WaitForSeconds(refreshRateSec);
        }
    }

    IEnumerator ShowPowerUp()
    {
        var powerUps = RaceManager.PowerUps;
        var currentPowerUp = powerUps.GetTargetPowerUp(Car);
        var oldRefreshRate = int.MaxValue;
        var refreshRateSec = 1f;
        Coroutine _lastCooldown = null;

        while (true)
        {
            var pUp = powerUps.GetTargetPowerUp(Car);
            if (currentPowerUp != pUp)
            {
                PowerUp.gameObject.SetActive(false);
                PowerUpFill.gameObject.SetActive(false);
                PowerUpText.gameObject.SetActive(false);

                if (_lastCooldown != null)
                    StopCoroutine(_lastCooldown);

                if(pUp != null)
                    _lastCooldown = StartCoroutine(ShowPowerUpCooldown(pUp));

                currentPowerUp = pUp;
            }

            if (oldRefreshRate != PowerUpRefreshRate)
            {
                oldRefreshRate = PowerUpRefreshRate;
                refreshRateSec = 1f / PowerUpRefreshRate;
            }

            yield return new WaitForSeconds(refreshRateSec);
        }
    }

    IEnumerator ShowPowerUpCooldown(PowerUp powerup)
    {
        var timeLeft = powerup.Duration;
        var invDuration = 1f / powerup.Duration;

        PowerUp.gameObject.SetActive(true);
        PowerUpFill.gameObject.SetActive(true);
        PowerUpText.gameObject.SetActive(true);

        PowerUp.color = powerup.AccentColor;
        PowerUpFill.color = powerup.AccentColor;
        PowerUpText.color = powerup.AccentColor;
        PowerUpText.text = powerup.Description;

        while (timeLeft > 0)
        {
            PowerUpFill.fillAmount = invDuration * timeLeft;
            timeLeft -= Time.deltaTime;
            yield return null;
        }
    }
}
