using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CarUI : MonoBehaviour
{
    public GameObject CarObject;

    public Slider SpeedSlider;
    public Image SpeedSliderFill;
    public Text SpeedText;
    public Toggle InTrackToggle;
    public Text LapsText;
    public Text NextCheckpoint;
    public Color OnBoostColor;
    public Text CurrentLapTime;
    public RectTransform PartialsRect;
    public RectTransform PartialPrefab;
    public List<Text> Partials;

    [SerializeField]
    Car _car;
    [SerializeField]
    LapCounter _carTracker;
    Color _oldSpeedSliderFillColor;
    [SerializeField]
    RaceManager _raceManager;
    [SerializeField]
    int _usedPartials;

    void Start()
    {
        _raceManager = RaceManager.Instance;
        _car = CarObject.GetComponent<Car>();
        _carTracker = CarObject.GetComponent<LapCounter>();

        SpeedSlider.minValue = 0;
        SpeedSlider.wholeNumbers = true;
        _oldSpeedSliderFillColor = SpeedSliderFill.color;
        _usedPartials = 0;
    }

    void Update()
    {
        UpdateTrackStats();
        UpdateRaceStats();
        UpdateCarSpeedInfo();
        UpdateCarTimeStats();
    }

    void UpdateTrackStats()
    {
        InTrackToggle.isOn = _car.CarMovement.InTrack;
    }

    void UpdateRaceStats()
    {
        LapsText.text = string.Concat("Lap: ", _carTracker.CurrentLap);
        NextCheckpoint.text = string.Concat("Next checkpoint: ", _carTracker.CurrentCheckpoint);
    }

    void UpdateCarSpeedInfo()
    {
        var carMovement = _car.CarMovement;
        SpeedSlider.maxValue = carMovement.TopSpeedKMH;
        if (carMovement.InTrack || _raceManager.RaceIsOn)
        {
            SpeedSlider.value = carMovement.SpeedKMH;
            SpeedText.text = ((int)carMovement.SpeedKMH).ToString();
        }
        else
        {
            SpeedSlider.value = 0;
            SpeedText.text = string.Concat("0");
        }

        if (carMovement.PowerUp != null && carMovement.PowerUp.Type == PowerUp.Types.BOOST)
            SpeedSliderFill.color = Color.Lerp(SpeedSliderFill.color, OnBoostColor, Time.unscaledDeltaTime * 5f);
        else
            SpeedSliderFill.color = Color.Lerp(SpeedSliderFill.color, _oldSpeedSliderFillColor, Time.unscaledDeltaTime * 5f);
    }

    void UpdateCarTimeStats()
    {
        var carTimeCounter = _car.LapTimeCounter;
        CurrentLapTime.text = string.Concat("Lap time: ", Utils.GetCounterFormattedString(carTimeCounter.CurrentLapTime));

        var carLapCounter = _car.LapCounter;
        if (_raceManager.RaceIsOn && carTimeCounter.PartialTimes.Count > 0)
        {
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
                    }

                    var partial = Partials[i];
                    partial.text = string.Concat(Utils.GetCounterFormattedString(currentLapPartials[i]));
                    partial.gameObject.SetActive(true);
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
            }
        }
    }

    Text CreatePartial()
    {
        var partial = new GameObject().AddComponent<RectTransform>();
        partial.SetParent(PartialsRect);
        partial.name = "Lap Partial";

        _usedPartials++;

        return partial.gameObject.AddComponent<Text>();
    }
}
