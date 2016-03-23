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
    public CheckpointManager CheckpointManager;
    public bool RandomCarSpot;
    public RaceTypes RaceType;
    public List<Checkpoint> Checkpoints;
    public int StartCheckpoint;
    public List<Transform> StartingPositions;
    public RaceState State;
    public Countdown Countdown;
    public int RaceLaps;
    [Range(1, 60)]
    public int CheckWinnerRate;

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
                if (car.LapTimeCounter.LapsTimes.Count >= RaceLaps)
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
                StartCoroutine(OnRaceFinished());
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

    public int GetFirstCheckpoint()
    {
        return StartCheckpoint;
    }

    public int GetNextCheckpoint(int currentCheckpoint)
    {
        return (currentCheckpoint + 1) % Checkpoints.Count;
    }

    public bool IsColliderOfCheckpoint(Collider col, int checkpointIndex)
    {
        return Checkpoints[checkpointIndex].Trigger == col;
    }

    public int GetCurrentLap(int passedCheckpoints)
    {
        if (passedCheckpoints == 0)
            return -1;

        // account for first checkpoint, which is always passed in the beggining
        return ((passedCheckpoints - 1) / Checkpoints.Count);
    }

    public void SetPaused(bool value)
    {
        State.Paused = value;
        Time.timeScale = value ? 0 : 1;
    }

    IEnumerator OnRaceFinished()
    {
        State.Finished = true;
        foreach(var car in CarsManager.Cars)
        {
            car.CarMovement.State.CanMove = false;
        }

        while (Time.timeScale > 0.025f)
        {
            yield return new WaitForEndOfFrame();

            Time.timeScale = Mathf.Lerp(Time.timeScale, 0, Time.unscaledDeltaTime * 10);
        }

        Time.timeScale = 0;
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