using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(LapCounter))]
[RequireComponent(typeof(LapTimeCounter))]
public class Car : MonoBehaviour
{
	public int CarId;

    public ICollection<Upgrade> Upgrades;

    public float TopSpeedInKmh;
    public float MassInKg;
    CarMovement _carMovement;
    LapCounter _lapCounter;
    LapTimeCounter _timeCounter;

    public CarMovement CarMovement
    {
        get
        {
            if (_carMovement == null)
                _carMovement = GetComponent<CarMovement>();
            return _carMovement;
        }
    }

    public LapCounter LapCounter
    {
        get
        {
            if (_lapCounter == null)
                _lapCounter = GetComponent<LapCounter>();
            return _lapCounter;
        }
    }

    public LapTimeCounter LapTimeCounter
    {
        get
        {
            if (_timeCounter == null)
                _timeCounter = GetComponent<LapTimeCounter>();
            return _timeCounter;
        }
    }

    void Start()
    {
        Upgrades = new List<Upgrade>();
    }

    public void ApplyUpgrades()
    {
        foreach (var upgrade in Upgrades)
        {
            upgrade.Apply(this);
        }
    }

    public void AddUpgrade(Upgrade upgrade)
    {
        Upgrades.Add(upgrade);
    }
}