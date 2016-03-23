using UnityEngine;
using System.Collections;

public class TopSpeed : PowerUp
{
    public float TopSpeedFactor;

    Coroutine _applied;

    protected override void Start()
    {
        base.Start();

        Type = Types.TopSpeedBoost;
    }

    protected override bool ApplyEffects()
    {
        var carMovement = Target.CarMovement;
        if (carMovement == null)
        {
            Debug.LogWarning("Car movement is null! Power-up [Top Speed] cannot be applied.");
            return false;
        }

        var carSpeedStats = carMovement.SpeedStatsKMH;

        carSpeedStats.CurrentTopSpeed = carSpeedStats.TopSpeed * TopSpeedFactor;
        carSpeedStats.CurrentTopSpeedReverse = carSpeedStats.TopSpeedReverse * TopSpeedFactor;

        return true;
    }

    protected override void RemoveEffects()
    {
        var carMovement = Target.CarMovement;
        if (carMovement == null)
        {
            Debug.LogWarning("Car movement is null! Power-up [Top Speed] cannot be applied.");
            return;
        }

        var carSpeedStats = carMovement.SpeedStatsKMH;
        carSpeedStats.CurrentTopSpeed = carSpeedStats.TopSpeed;
        carSpeedStats.CurrentTopSpeedReverse = carSpeedStats.TopSpeedReverse;
    }
}
