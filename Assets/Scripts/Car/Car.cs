using Assets.Scripts.Car.Upgrades;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Car
{
    public class Car : MonoBehaviour
    {
		public int CarId;

        public ICollection<Upgrade> Upgrades;

        public float TopSpeedInKmh;
        public float MassInKg;

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