using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Car;

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

    public List<TrackTrigger> Checkpoints;
    public int StartCheckpoint;
    public List<Transform> StartingPositions;
    public RaceTypes RaceType;
    public List<Car> Cars;
    public int CountdownCount, CurrentCount;
    public bool RaceIsOn;

    Coroutine _lastCountdown;

    void Start()
    {
        RaceIsOn = false;
    }

    public void NewRace()
    {
        RaceIsOn = false;

        for (int i = 0; i < Cars.Count; i++)
        {
            var car = Cars[i];
            car.transform.position = StartingPositions[i].position;
            car.transform.rotation = StartingPositions[i].rotation;
            car.CarMovement.CanMove = false;
        }

        if (_lastCountdown != null)
            StopCoroutine(_lastCountdown);
        _lastCountdown = StartCoroutine(CountdownAndStart());
    }

    IEnumerator CountdownAndStart()
    {
        RaceIsOn = true;

        CurrentCount = CountdownCount;
        float timePassed = CountdownCount;

        while (timePassed >= 0)
        {
            if (timePassed <= (CurrentCount - 1))
                CurrentCount--;

            timePassed -= Time.deltaTime;
            yield return null;
        }

        foreach (var car in Cars)
        {
            car.CarMovement.Velocity = Vector3.zero;
            car.CarMovement.CanMove = true;
        }

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
            return 0;

        // account for first checkpoint, which is always passed in the beggining
        return ((passedCheckpoints - 1) / Checkpoints.Count) + 1;
    }
}
