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

    [SerializeField]
    bool _timeCounting;
    [SerializeField]
    Car _car;
    List<List<float>> _partialTimes;

    List<List<float>> PartialTimes
    {
        get
        {
            if (_partialTimes == null)
                _partialTimes = new List<List<float>>();

            return _partialTimes;
        }
    }

    public List<float> CurrentLapPartials
    {
        get
        {
            if (PartialTimes.Count > 0)
            {
                var currentLap = _car.LapCounter.CurrentLap;
                if (PartialTimes.Count > currentLap)
                {
                    return PartialTimes[currentLap];
                }
            }

            return null;
        }
    }

    public float RaceTime
    {
        get
        {
            var sum = 0f;
            foreach (var time in LapsTimes)
                sum += time;

            return sum;
        }
    }

    void Start()
    {
        _car = GetComponent<Car>();
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
            PartialTimes[_car.LapCounter.CurrentLap].Add(CurrentLapTime);
        }
    }

    public float GetPartial(int lap, int checkpoint)
    {
        return PartialTimes[lap][checkpoint];
    }

    public bool IsBestPartial(int checkpoint, float value)
    {
        for(int i = 0; i < PartialTimes.Count - 1; i++)
        {
            if (PartialTimes[i][checkpoint] <= value)
                return false;
        }

        return true;
    }
}
