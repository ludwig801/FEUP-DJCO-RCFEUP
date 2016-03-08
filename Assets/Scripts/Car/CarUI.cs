using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CarUI : MonoBehaviour
{
    public GameObject CarObject;

    public Toggle InTrackToggle;
    public Text Speed;
    public Text CurrentLap;
    public Text NextCheckpoint;
    public Color OnBoostColor;
    public Text CurrentLapTime;
    public RectTransform PartialsRect, BestPartialsRect;
    public RectTransform PartialPrefab;
    public List<Text> Partials;
    public List<Text> BestPartials;
    public Color BestPartialColor, WorstPartialColor;
    public Image CurrentPowerUp, CurrentPowerUpMask, CurrentPowerUpDuration;
    public Sprite PowerUpDefaultSprite;
    public Color PowerUpDurationColor;

    [SerializeField]
    Car _car;
    [SerializeField]
    LapCounter _carLapCounter;
    [SerializeField]
    RaceManager _raceManager;

    void Start()
    {
        _raceManager = RaceManager.Instance;
        _car = CarObject.GetComponent<Car>();
        _carLapCounter = CarObject.GetComponent<LapCounter>();
    }

    void Update()
    {
        UpdateTrackStats();
        UpdateRaceStats();
        UpdateCarTimeStats();
        UpdateCarPowerUp();

        Speed.text = _car.CarMovement.AngularVelocity.magnitude.ToString();
    }

    void UpdateTrackStats()
    {
        InTrackToggle.isOn = _car.CarMovement.InTrack;
    }

    void UpdateRaceStats()
    {
        CurrentLap.text = string.Concat("Lap: ", _carLapCounter.CurrentLapPlusOne);
        NextCheckpoint.text = string.Concat("Next checkpoint: ", _carLapCounter.CurrentCheckpoint);
    }

    void UpdateCarTimeStats()
    {
        var carTimeCounter = _car.LapTimeCounter;
        CurrentLapTime.text = string.Concat("Lap time: ", Utils.GetCounterFormattedString(carTimeCounter.CurrentLapTime));

        if (_raceManager.RaceIsOn && carTimeCounter.PartialTimes.Count > 0)
        {
            var currentLap = _car.LapCounter.CurrentLap;
            var currentLapPartials = carTimeCounter.CurrentLapPartials;
            if (currentLapPartials != null)
            {
                for (int i = 0; i < currentLapPartials.Count; i++)
                {
                    if (Partials.Count <= i)
                    {
                        var tPartial = Instantiate(PartialPrefab);
                        tPartial.SetParent(PartialsRect);
                        tPartial.name = string.Concat("Partial ", i);
                        Partials.Add(tPartial.GetComponent<Text>());
                        var bPartial = Instantiate(PartialPrefab);
                        bPartial.SetParent(BestPartialsRect);
                        bPartial.name = string.Concat("Best ", i);
                        BestPartials.Add(bPartial.GetComponent<Text>());
                    }

                    var partialTime = currentLapPartials[i];
                    var partialText = Partials[i];
                    var best = true;
                    if (currentLap > 0)
                    {
                        for (int j = 0; j < currentLap; j++)
                        {
                            if (partialTime > carTimeCounter.GetPartial(j, i))
                            {
                                best = false;
                                break;
                            }
                        }
                    }

                    partialText.text = string.Concat(Utils.GetCounterFormattedString(partialTime));
                    partialText.color = best ? BestPartialColor : WorstPartialColor;
                    if (best)
                    {
                        var bestPartialText = BestPartials[i];
                        bestPartialText.text = partialText.text;
                        bestPartialText.color = partialText.color;
                        bestPartialText.gameObject.SetActive(true);
                    }
                    partialText.gameObject.SetActive(true);
                }

                for (int i = currentLapPartials.Count; i < Partials.Count; i++)
                {
                    Partials[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < Partials.Count; i++)
            {
                Partials[i].gameObject.SetActive(false);
                BestPartials[i].gameObject.SetActive(false);
            }
        }
    }

    void UpdateCarPowerUp()
    {
        var carMovement = _car.CarMovement;
        if (carMovement.PowerUp != null)
        {
            var powerUp = carMovement.PowerUp;
            CurrentPowerUp.enabled = true;
            CurrentPowerUp.sprite = powerUp.Sprite;
            CurrentPowerUpMask.color = Color.Lerp(CurrentPowerUpMask.color, powerUp.AccentColor, Time.deltaTime * 5f);
            CurrentPowerUpDuration.color = Color.Lerp(CurrentPowerUpDuration.color, PowerUpDurationColor, Time.deltaTime * 5f);
            CurrentPowerUpDuration.fillAmount = powerUp.TimeLeft / powerUp.Duration;
        }
        else
        {
            CurrentPowerUp.enabled = false;
            CurrentPowerUp.sprite = PowerUpDefaultSprite;
            CurrentPowerUpMask.color = Color.Lerp(CurrentPowerUpMask.color, Color.grey, Time.deltaTime * 5f);
            CurrentPowerUpDuration.color = Color.Lerp(CurrentPowerUpDuration.color, Color.grey, Time.deltaTime * 5f);
            CurrentPowerUpDuration.fillAmount = 1;
        }
    }

    Text CreatePartial()
    {
        var partial = new GameObject().AddComponent<RectTransform>();
        partial.SetParent(PartialsRect);
        partial.name = "Lap Partial";

        return partial.gameObject.AddComponent<Text>();
    }
}
