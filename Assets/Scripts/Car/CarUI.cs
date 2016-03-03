using UnityEngine;
using UnityEngine.UI;

public class CarUI : MonoBehaviour
{
    public GameObject CarObject;

    public Slider SpeedSlider;
    public Text SpeedText;
    public Toggle InTrackToggle;
    public Text LapsText;
    public Text NextCheckpoint;

    [SerializeField]
    CarMovement _carMovement;
    [SerializeField]
    CarTracker _carTracker;

    void Start()
    {
        _carMovement = CarObject.GetComponent<CarMovement>();
        _carTracker = CarObject.GetComponent<CarTracker>();

        SpeedSlider.minValue = 0;
        SpeedSlider.wholeNumbers = true;
    }
   
    void Update()
    {
        InTrackToggle.isOn = _carMovement.InTrack;

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

        LapsText.text = string.Concat("Lap: ", _carTracker.CurrentLap);
        NextCheckpoint.text = string.Concat("Next checkpoint: ", _carTracker.NextCheckpoint);
    }
}
