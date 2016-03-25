using UnityEngine;

[RequireComponent(typeof(Car))]
public class LapCounter : MonoBehaviour
{
    public AudioSource PassAudio;

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
        PassAudio.pitch = 0.5f;

        Reset();
    }

    public void Reset()
    {
        PassedCheckpoints = 0;
        CurrentCheckpoint = RaceManager.CheckpointManager.GetStartingCheckpoint();
        _currentLap = RaceManager.GetCurrentLap(PassedCheckpoints);
    }

    void OnTriggerEnter(Collider other)
    {
        if (RaceManager.CheckpointManager.GetCheckpointIndex(other) == CurrentCheckpoint)
        {
            _car.LapTimeCounter.OnCheckpointPassed(CurrentCheckpoint);
            CurrentCheckpoint = RaceManager.CheckpointManager.GetNextCheckpoint(CurrentCheckpoint);
            PassedCheckpoints++;
            _currentLap = RaceManager.GetCurrentLap(PassedCheckpoints);

            PassAudio.Play();
        }
    }
}
