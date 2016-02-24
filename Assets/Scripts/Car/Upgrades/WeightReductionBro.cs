using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Car.Upgrades
{
    public class WeightReductionBro : Upgrade
    {
        public float WeightReduction;

        public override void Apply(CarMovement car)
        {
            car.ReduceMass(500);
        }

        public override void Remove()
        {
            throw new NotImplementedException();
        }
    }
}
