using UnityEngine;
using System.Collections.Generic;
using System;

public class CarMovement : MonoBehaviour
{
    public List<AxleInfo> AxleInfos;
    public float maxEngineTorque;
    public float maxSteeringAngle;

    Rigidbody _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = 0.1f * _rigidbody.centerOfMass;
    }

    void FixedUpdate()
    {
        float engineTorque = maxEngineTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        float brake = Input.GetKey(KeyCode.Space) ? _rigidbody.mass * 0.1f : 0;
        engineTorque = brake > 0 ? 0 : engineTorque;

        foreach (AxleInfo axleInfo in AxleInfos)
        {
            if (axleInfo.steering)
            {
                if (axleInfo.inverseSteering)
                {
                    axleInfo.leftWheel.steerAngle = -steering;
                    axleInfo.rightWheel.steerAngle = -steering;
                }
                else
                {
                    axleInfo.leftWheel.steerAngle = steering;
                    axleInfo.rightWheel.steerAngle = steering;
                }
            }

            if (axleInfo.engine)
            {
                axleInfo.leftWheel.motorTorque = engineTorque;
                axleInfo.rightWheel.motorTorque = engineTorque;
            }

            axleInfo.leftWheel.brakeTorque = brake;
            axleInfo.rightWheel.brakeTorque = brake;
        }
    }

    [Serializable]
    public class AxleInfo
    {
        public WheelCollider leftWheel, rightWheel;
        public bool engine;
        public bool steering;
        public bool inverseSteering;
    }
}
