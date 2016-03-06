using System;

public class EngineUpgrade : Upgrade
{
    public int TopSpeedIncrement;

    public override void Apply(CarMovement carMovement)
    {
        carMovement.TopSpeedKMH += TopSpeedIncrement;
    }

    public override void Remove()
    {
        throw new NotImplementedException();
    }
}
