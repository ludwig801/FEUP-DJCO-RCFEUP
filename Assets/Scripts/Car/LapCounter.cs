using UnityEngine;

[RequireComponent(typeof(Car))]
public class LapCounter : MonoBehaviour
{
    public RaceManager RaceManager;

    [SerializeField]
    int _currentLap;
    [SerializeField]
    Car _car;

    public int CurrentLapPlusOne
    {
        get
        {
            return _currentLap + 1;
        }
    }

    public int CurrentLap
    {
        get
        {
            return _currentLap;
        }
    }

    public int PassedCheckpoints
    {
        get;
        private set;
    }

    public int CurrentCheckpoint
    {
        get;
        private set;
    }

    void Start()
    {
        RaceManager = RaceManager.Instance;
        _car = GetComponent<Car>();

        Reset();
    }

    public void Reset()
    {
        PassedCheckpoints = 0;
        CurrentCheckpoint = RaceManager.Checkpoints.GetStartingCheckpoint();
        _currentLap = RaceManager.GetCurrentLap(PassedCheckpoints);
    }

    void OnTriggerEnter(Collider other)
    {
        if (RaceManager.Checkpoints.GetCheckpointIndex(other) == CurrentCheckpoint)
        {
            _car.LapTimeCounter.OnCheckpointPassed(CurrentCheckpoint);
            CurrentCheckpoint = RaceManager.Checkpoints.GetNextCheckpoint(CurrentCheckpoint);
            PassedCheckpoints++;
            _currentLap = RaceManager.GetCurrentLap(PassedCheckpoints);
        }
    }
}
