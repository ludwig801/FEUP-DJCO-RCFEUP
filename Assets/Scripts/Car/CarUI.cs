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

    [SerializeField]
    CarMovement _carMovement;
    [SerializeField]
    LapCounter _carTracker;
    Color _oldSpeedSliderFillColor;

    void Start()
    {
        _carMovement = CarObject.GetComponent<CarMovement>();
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
    }

    void UpdateTrackStats()
    {
        InTrackToggle.isOn = _carMovement.InTrack;
    }

    void UpdateRaceStats()
    {
        LapsText.text = string.Concat("Lap: ", _carTracker.CurrentLap);
        NextCheckpoint.text = string.Concat("Next checkpoint: ", _carTracker.NextCheckpoint);
    }

    void UpdateCarSpeedInfo()
    {
        SpeedSlider.maxValue = _carMovement.TopSpeedKMH;
        if (_carMovement.InTrack)
        {
            SpeedSlider.value = _carMovement.SpeedKMH;
            SpeedText.text = ((int)_carMovement.SpeedKMH).ToString();
        }
        else
        {
            SpeedSlider.value = 0;
            SpeedText.text = string.Concat("0");
        }

        if (_carMovement.PowerUp != null && _carMovement.PowerUp.Type == PowerUp.Types.BOOST)
            SpeedSliderFill.color = Color.Lerp(SpeedSliderFill.color, OnBoostColor, Time.unscaledDeltaTime * 5f);
        else
            SpeedSliderFill.color = Color.Lerp(SpeedSliderFill.color, _oldSpeedSliderFillColor, Time.unscaledDeltaTime * 5f);
    }
}
