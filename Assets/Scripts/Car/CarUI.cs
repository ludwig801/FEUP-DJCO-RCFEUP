using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class CarUI : MonoBehaviour
{
    public int CarIndex;
    public RectTransform LapPartialPrefab;
    public Text Checkpoint, Lap, LapTime;
    public RectTransform LapPartialsRect, BestPartialsRect;
    public Color BestPartialColor, WorstPartialColor;
    public Image PowerUp, PowerUpBackground, PowerUpFill;
    public bool RunLapStats, RunTimeStats, RunPowerUpStats;

    private Car _car;
    private RaceManager _raceManager;
    private List<Text> _lapPartials, _bestPartials;

    void Start()
    {
        _raceManager = RaceManager.Instance;
        _car = _raceManager.Cars[CarIndex];
        _lapPartials = new List<Text>();
        _bestPartials = new List<Text>();

        StartCoroutine(ShowLapStats());
        StartCoroutine(ShowTimeStats());
    }

    IEnumerator ShowLapStats()
    {
        var oldLap = -1;
        var oldCheckpoint = -1;
        var lapCounter = _car.LapCounter;

        if (Lap == null || Checkpoint == null)
        {
            Debug.LogWarning(string.Concat(Lap.name, " or ", Checkpoint.name, " is null. Coroutine will not proceed and UI values will not be updated."));
            yield break;
        }

        while (true)
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

            yield return null;
        }
    }

    IEnumerator ShowTimeStats()
    {
        var oldCheckpoint = -1;
        var lapCounter = _car.LapCounter;
        var lapTimeCounter = _car.LapTimeCounter;
        var oldPartialsCount = 0;

        while (true)
        {
            if (_raceManager.RaceIsOn && lapTimeCounter.CurrentLapPartials != null)
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

            yield return null;
        }
    }

    void UpdateCarPowerUp()
    {
        var carMovement = _car.CarMovement;
        if (carMovement.PowerUp != null)
        {
            var powerUp = carMovement.PowerUp;
            PowerUp.enabled = true;
            PowerUp.sprite = powerUp.Sprite;
            PowerUpFill.color = Color.Lerp(PowerUpFill.color, powerUp.AccentColor, Time.deltaTime * 5f);
            PowerUpFill.fillAmount = powerUp.TimeLeft / powerUp.Duration;
        }
        else
        {
            PowerUp.enabled = false;
            PowerUpFill.fillAmount = 0;
        }
    }
}
