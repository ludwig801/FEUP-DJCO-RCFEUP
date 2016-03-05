using Assets.Scripts.Car.Upgrades;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Car
{
    [RequireComponent(typeof(CarMovement))]
    [RequireComponent(typeof(LapCounter))]
    public class Car : MonoBehaviour
    {
		public int CarId;

        public ICollection<Upgrade> Upgrades;

        public float TopSpeedInKmh;
        public float MassInKg;
        CarMovement _carMovement;
        LapCounter _lapCounter;

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
}