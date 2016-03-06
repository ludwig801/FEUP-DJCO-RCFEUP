using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Car))]
public class LapTimeCounter : MonoBehaviour
{
    public float StartTime;
    public float TotalTime;
    public float CurrentLapStartTime;
    public float CurrentLapTime;
    public List<float> LapsTimes;
    public List<List<float>> PartialTimes;

    [SerializeField]
    bool _timeCounting;
    [SerializeField]
    Car _car;

    public List<float> CurrentLapPartials
    {
        get
        {
            if (PartialTimes.Count > 0)
            {
                var currentLap = _car.LapCounter.CurrentLapZeroIndexed;
                if (PartialTimes.Count > currentLap)
                {
                    return PartialTimes[currentLap];
                }
            }

            return null;
        }
    }

    void Start()
    {
        _car = GetComponent<Car>();
        PartialTimes = new List<List<float>>();
        Reset();
    }

    void Update()
    {
        if (_timeCounting)
        {
            TotalTime = Time.time - StartTime;
            CurrentLapTime = Time.time - CurrentLapStartTime;
        }
    }

    public void Reset()
    {
        TotalTime = 0;
        CurrentLapTime = 0;
        _timeCounting = false;
        LapsTimes.Clear();
        PartialTimes.Clear();
    }

    public void OnCheckpointPassed(int index)
    {
        if (index == 0)
        {
            if (!_timeCounting)
            {
                _timeCounting = true;
                StartTime = Time.time;
                TotalTime = 0;
                LapsTimes.Clear();
                PartialTimes.Clear();
            }
            else
            {
                LapsTimes.Add(CurrentLapTime);
            }

            CurrentLapStartTime = Time.time;
            CurrentLapTime = 0;
            PartialTimes.Add(new List<float>());
        }
        else
        {
            PartialTimes[_car.LapCounter.CurrentLapZeroIndexed].Add(CurrentLapTime);
        }
    }
}
