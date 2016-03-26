using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class CarUI : MonoBehaviour
{
    public RaceManager RaceManager;
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
    private Car _car;
    private bool _oldFinished;

    void Start()
    {
        RaceManager = RaceManager.Instance;
        _lapPartials = new List<Text>();
        _bestPartials = new List<Text>();
        _car = GetComponentInParent<CarCanvas>().Car;

        StartCoroutine(ShowLapStats());
        StartCoroutine(ShowTimeStats());
        StartCoroutine(ShowCoins());
        StartCoroutine(ShowPowerUp());

        _oldFinished = _car.Finished;
    }

    void Update()
    {
        if (_oldFinished != _car.Finished)
        {
            _oldFinished = _car.Finished;
            if (_car.Finished)
            {
                StopAllCoroutines();
            }
            else
            {
                StartCoroutine(ShowLapStats());
                StartCoroutine(ShowTimeStats());
                StartCoroutine(ShowCoins());
                StartCoroutine(ShowPowerUp());
            }

            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(!_car.Finished);
        }
    }

    IEnumerator ShowLapStats()
    {
        var oldLap = -1;
        var oldCheckpoint = -1;
        var lapCounter = _car.LapCounter;
        var oldRefreshRate = int.MaxValue;
        var refreshRateSec = 1f;
        var checkpointCount = RaceManager.CheckpointManager.Checkpoints.Count;
        var lapCount = RaceManager.NumLaps;

        if (Lap == null || Checkpoint == null)
        {
            Debug.LogWarning(string.Concat(Lap.name, " or ", Checkpoint.name, " is null. Coroutine will not proceed and UI values will not be updated."));
            yield break;
        }

        Checkpoint.text = string.Concat("- | ", checkpointCount);
        Lap.text = string.Concat("- | ", lapCount);

        while (true)
        {
            if (RunLapStats)
            {
                if (oldLap != lapCounter.CurrentLap)
                {
                    oldLap = lapCounter.CurrentLap;
                    Lap.text = string.Concat(lapCounter.CurrentLapPlusOne, " | ", lapCount);
                }

                if (oldCheckpoint != lapCounter.PassedCheckpoints)
                {
                    oldCheckpoint = lapCounter.PassedCheckpoints;
                    Checkpoint.text = string.Concat(lapCounter.PassedCheckpoints, " | ", checkpointCount);
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
        var lapCounter = _car.LapCounter;
        var lapTimeCounter = _car.LapTimeCounter;
        var oldPartialsCount = 0;
        var oldRefreshRate = int.MaxValue;
        var refreshRateSec = 1f;

        LapTime.text = string.Concat(Utils.GetCounterFormattedString(0));

        while (true)
        {
            if (RaceManager.Countdown.Running)
            {
                for (var i = 0; i < _lapPartials.Count; i++)
                {
                    _lapPartials[i].enabled = false;
                    _bestPartials[i].enabled = false;
                }
                LapTime.text = string.Concat(Utils.GetCounterFormattedString(0));
            }

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
                            partial.transform.SetParent(LapPartialsRect, false);
                            partial.name = string.Concat("Partial ", partialsCount);
                            _lapPartials.Add(partial);

                            partial = Instantiate(LapPartialPrefab).GetComponent<Text>();
                            partial.transform.SetParent(BestPartialsRect, false);
                            partial.name = string.Concat("Best ", partialsCount);
                            partial.enabled = true;
                            _bestPartials.Add(partial);
                        }

                        var checkpoint = partialsCount - 1;
                        var currentPartialText = _lapPartials[checkpoint];
                        var currentPartialValue = currentPartials[checkpoint];
                        currentPartialText.text = Utils.GetCounterFormattedString(currentPartialValue);
                        currentPartialText.enabled = true;
                        if(!_bestPartials[checkpoint].enabled)
                            _bestPartials[checkpoint].enabled = true;
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

                LapTime.text = string.Concat(Utils.GetCounterFormattedString(lapTimeCounter.CurrentLapTime));
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
        var oldRefreshRate = int.MaxValue;
        var refreshRateSec = 1f;

        while (true)
        {
            if (RunCollectablesStats)
            {
                if (oldCoinCount != _car.Coins)
                {
                    oldCoinCount = _car.Coins;
                    CoinCount.text = string.Concat(_car.Coins);
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
        var currentPowerUp = powerUps.GetTargetPowerUp(_car);
        var oldRefreshRate = int.MaxValue;
        var refreshRateSec = 1f;
        Coroutine _lastCooldown = null;

        while (true)
        {
            var pUp = powerUps.GetTargetPowerUp(_car);
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
