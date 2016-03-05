using System;
using UnityEngine;

[RequireComponent(typeof(RaceManager))]
public class TimeTrial : MonoBehaviour, IRaceType
{
    public int NumLaps;

    public bool IsRaceOver()
    {
        return false;
    }

    public void OnRaceStart()
    {
    }
}
