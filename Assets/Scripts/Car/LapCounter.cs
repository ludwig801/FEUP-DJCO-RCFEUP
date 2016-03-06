﻿using UnityEngine;

[RequireComponent(typeof(Car))]
public class LapCounter : MonoBehaviour
{
    [SerializeField]
    RaceManager _raceManager;
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
        _raceManager = RaceManager.Instance;
        _car = GetComponent<Car>();

        Reset();
    }

    public void Reset()
    {
        PassedCheckpoints = 0;
        CurrentCheckpoint = _raceManager.GetFirstCheckpoint();
        _currentLap = _raceManager.GetCurrentLap(PassedCheckpoints);
    }

    void OnTriggerEnter(Collider other)
    {
        if (_raceManager.IsColliderOfCheckpoint(other, CurrentCheckpoint))
        {
            _car.LapTimeCounter.OnCheckpointPassed(CurrentCheckpoint);
            CurrentCheckpoint = _raceManager.GetNextCheckpoint(CurrentCheckpoint);
            PassedCheckpoints++;
            _currentLap = _raceManager.GetCurrentLap(PassedCheckpoints);
        }
    }
}
