using UnityEngine;
using System.Collections;
using System;

public class TorqueBoost : PowerUp
{
    public float BoostFactor;

    Coroutine _applied;

    protected override void Start()
    {
        base.Start();
        Type = Types.TorqueBoost;
    }

    protected override bool ApplyEffects()
    {
        var carMovement = Target.CarMovement;
        if (carMovement == null)
        {
            Debug.LogWarning("Car movement is null! Power-up [Torque Boost] cannot be applied.");
            return false;
        }

        var carTorqueSystem = carMovement.TorqueSystem;

        carTorqueSystem.CurrentMotorTorque = carTorqueSystem.MotorTorque * BoostFactor;

        return true;
    }

    protected override void RemoveEffects()
    {
        var carMovement = Target.CarMovement;
        if (carMovement == null)
        {
            Debug.LogWarning("Car movement is null! Power-up [Torque Boost] cannot be applied.");
            return;
        }

        var carTorqueSystem = carMovement.TorqueSystem;
        carTorqueSystem.CurrentMotorTorque = carTorqueSystem.MotorTorque;
    }
}
