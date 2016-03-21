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

    public List<Checkpoint> Checkpoints;
    public int StartCheckpoint;
    public List<Transform> StartingPositions;
    public RaceTypes RaceType;
    public List<Car> Cars;
    public int FastestCarCurrentLap, RaceLaps;
    public bool RaceIsOn, RacePaused, CountdownIsOn, RaceIsFinished;
    public Car Winner;
    public int CountdownCount, CurrentCount;
    [Range(1, 60)]
    public int CheckWinnerRate;

    Coroutine _lastCountdown;
    [SerializeField]
    IRaceType _currentRace;

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
        RacePaused = false;
        RaceIsOn = false;
        RaceIsFinished = false;
        CountdownIsOn = false;
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

            foreach (var car in Cars)
            {
                if (car.LapTimeCounter.LapsTimes.Count >= RaceLaps)
                {
                    var carTime = car.LapTimeCounter.TotalTime;
                    if (carTime < time)
                    {
                        time = carTime;
                        Winner = car;
                    }
                }
            }

            if (Winner != null || RaceIsFinished)
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

        yield break;
    }

    public void NewRace()
    {
        RaceIsOn = false;
        CountdownIsOn = false;
        RaceIsFinished = false;
        FastestCarCurrentLap = 0;

        for (int i = 0; i < Cars.Count; i++)
        {
            var car = Cars[i];
            car.transform.position = StartingPositions[i].position;
            car.transform.rotation = StartingPositions[i].rotation;
            car.CarMovement.State.CanMove = false;
            car.LapCounter.Reset();
            car.LapTimeCounter.Reset();
        }

        switch (RaceType)
        {
            case RaceTypes.TIME_TRIAL:
                _currentRace = GetComponent<TimeTrial>() as IRaceType;
                break;
        }

        if (_lastCountdown != null)
            StopCoroutine(_lastCountdown);

        _lastCountdown = StartCoroutine(CountdownAndStart());
    }

    IEnumerator CountdownAndStart()
    {
        RaceIsOn = true;
        RacePaused = false;
        CountdownIsOn = true;
        _currentRace.OnRaceStart();

        CurrentCount = CountdownCount;
        float timePassed = CountdownCount;

        while (CurrentCount != 0)
        {
            yield return new WaitForSeconds(1);
            CurrentCount--;
        }

        CurrentCount = 0;
        CountdownIsOn = false;

        foreach (var car in Cars)
            car.CarMovement.State.CanMove = true;

        yield break;
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
        return Checkpoints[checkpointIndex].TriggerCollider == col;
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
        RacePaused = value;
        Time.timeScale = value ? 0 : 1;
    }

    IEnumerator OnRaceFinished()
    {
        RaceIsFinished = true;
        foreach(var car in Cars)
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
