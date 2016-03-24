using UnityEngine;

[RequireComponent(typeof(Car))]
public class LapCounter : MonoBehaviour
{
    [SerializeField]
    int _currentLap;
    [SerializeField]
    Car _car;

    RaceManager _raceManager;

    RaceManager RaceManager
    {
        get
        {
            if (_raceManager == null)
                _raceManager = RaceManager.Instance;

            return _raceManager;
        }
    }

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
