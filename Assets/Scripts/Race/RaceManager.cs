using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<RaceManager>();

            return _instance;
        }
    }
    static RaceManager _instance;

    public enum RaceTypes
    {
        TIME_TRIAL = 0
    }

    public PowerUpManager PowerUps;
    public CarsManager CarsManager;
    public CheckpointManager Checkpoints;
    public RaceTypes RaceType;
    public int NumLaps;
    [Range(1, 60)]
    public int CheckWinnerRate;
    public bool RandomCarSpot;
    public List<Transform> StartingPositions;
    public RaceState State;
    public Countdown Countdown;

    Coroutine _lastCountdown;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
    }

    void Start()
    {
        State.Reset();
        NewRace();

        StartCoroutine(CheckForWinner());
    }

    IEnumerator CheckForWinner()
    {
        var oldRefreshRate = int.MaxValue;
        var refresRateSec = 1f;

        while (true)
        {
            var time = float.MaxValue;

            foreach (var car in CarsManager.Cars)
            {
                if (car.LapTimeCounter.LapsTimes.Count >= NumLaps)
                {
                    var carTime = car.LapTimeCounter.TotalTime;
                    if (carTime < time)
                    {
                        time = carTime;
                        State.Winner = car;
                    }
                }
            }

            if (State.Winner != null || State.Finished)
            {
                OnRaceFinished();
                yield break;
            }

            if (oldRefreshRate != CheckWinnerRate)
            {
                oldRefreshRate = CheckWinnerRate;
                refresRateSec = 1f / CheckWinnerRate;
            }

            yield return new WaitForSeconds(refresRateSec); 
        }
    }

    public void NewRace()
    {
        State.Reset();
        PowerUps.ResetAllPowerUps();
        CarsManager.ResetAllCars();
        CarsManager.AssignStartingPositions(StartingPositions, RandomCarSpot);

        if (_lastCountdown != null)
            StopCoroutine(_lastCountdown);

        _lastCountdown = StartCoroutine(CountdownAndStart());
    }

    IEnumerator CountdownAndStart()
    {
        Countdown.Running = true;
        Countdown.CurrentCount = Countdown.Duration;

        while (Countdown.CurrentCount != 0)
        {
            yield return new WaitForSeconds(1);
            Countdown.CurrentCount--;
        }

        Countdown.CurrentCount = 0;
        Countdown.Running = false;
        State.Ongoing = true;

        CarsManager.ReleaseAllCars();
    }

    public int GetCurrentLap(int passedCheckpoints)
    {
        if (passedCheckpoints == 0)
            return -1;

        // account for first checkpoint, which is always passed in the beggining
        return ((passedCheckpoints - 1) / Checkpoints.Checkpoints.Count);
    }

    public void SetPaused(bool value)
    {
        State.Paused = value;
        Time.timeScale = value ? 0 : 1;
    }

    void OnRaceFinished()
    {
        State.Finished = true;
        CarsManager.ResetAllCars();
    }
}

[System.Serializable]
public class RaceState
{
    public bool Ongoing, Paused, Finished;
    public Car Winner;

    internal void Reset()
    {
        Paused = false;
        Ongoing = false;
        Finished = false;
        Winner = null;
    }
}

[System.Serializable]
public class Countdown
{
    [Range(1, 5)]
    public int Duration;
    public int CurrentCount;
    public bool Running;

    internal void Reset()
    {
        Running = false;
    }
}