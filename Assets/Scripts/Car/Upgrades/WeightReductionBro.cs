using System;

namespace Assets.Scripts.Car.Upgrades
{
    public class WeightReductionBro : Upgrade
    {
        public float WeightReductionInKg;

        public override void Apply(Car car)
        {
            car.MassInKg -= WeightReductionInKg;
        }

        public override void Remove()
        {
            throw new NotImplementedException();
        }
    }
}
