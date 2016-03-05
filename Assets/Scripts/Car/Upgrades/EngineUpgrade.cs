using System;

public class EngineUpgrade : Upgrade
{
    public int TopSpeedIncrement;

    public override void Apply(Car car)
    {
        car.TopSpeedInKmh += TopSpeedIncrement;
    }

    public override void Remove()
    {
        throw new NotImplementedException();
    }
}
