using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Car;
public class CarMovement : MonoBehaviour
{
    public float Speed;
    public float RotationSpeed;
    public Transform CenterOfMass;

    [SerializeField]
    Rigidbody _rigidbody;

    float SpeedKMH
    {
        get
        {
            return UnitConverter.VelocityToKmh(_rigidbody.velocity.magnitude);
        }
    }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        //_rigidbody.centerOfMass = CenterOfMass.position;
    }

    void FixedUpdate()
    {
        var vInput = Input.GetAxis("Vertical");
        var handbrake = Input.GetAxis("Jump");
        var hInput = Input.GetAxis("Horizontal");

        Move(vInput, vInput, handbrake, hInput);
    }

    public void ReduceMass(float value)
    {
        _rigidbody.mass -= value;
    }

    public void Move(float throttle, float footbrake, float handbrake, float steering)
    {
        throttle = Input.GetAxisRaw("Vertical");
        steering = Input.GetAxisRaw("Horizontal");
        handbrake = Mathf.Clamp(handbrake, 0, 1);

        _rigidbody.AddForce(throttle * transform.forward * Speed * _rigidbody.mass);
        _rigidbody.AddTorque(steering * transform.up * RotationSpeed * _rigidbody.mass);
        _rigidbody.angularVelocity = new Vector3(_rigidbody.angularVelocity.x, Mathf.Clamp(_rigidbody.angularVelocity.y, -3, 3), _rigidbody.angularVelocity.z);
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider LeftWheel;
    public WheelCollider RightWheel;
    public bool Motor;
    public bool Steering;
}