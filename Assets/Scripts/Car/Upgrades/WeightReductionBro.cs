using System;

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
