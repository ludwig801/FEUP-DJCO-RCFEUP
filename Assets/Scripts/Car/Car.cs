using Assets.Scripts.Car.Upgrades;
using System.Collections.Generic;

namespace Assets.Scripts.Car
{
    public class Car
    {
        public ICollection<Upgrade> Upgrades
        {
            get; set;
        }

        public Car()
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
    }
}
