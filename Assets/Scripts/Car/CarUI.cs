using Assets.Scripts.Car;
using UnityEngine;
using UnityEngine.UI;

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
    public RectTransform LapParcials;

    [SerializeField]
    Car _car;
    [SerializeField]
    LapCounter _carTracker;
    Color _oldSpeedSliderFillColor;
    [SerializeField]
    RaceManager _raceManager;

    void Start()
    {
        _raceManager = RaceManager.Instance;
        _car = CarObject.GetComponent<Car>();
        _carTracker = CarObject.GetComponent<LapCounter>();

        SpeedSlider.minValue = 0;
        SpeedSlider.wholeNumbers = true;
        _oldSpeedSliderFillColor = SpeedSliderFill.color;
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
    }
}
