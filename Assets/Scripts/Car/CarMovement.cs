using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Car;
using System.Collections.Generic;

public class CarMovement : MonoBehaviour
{
    public List<AxleInfo> Axles;
    public float Acceleration;
    public float TopSpeed;
    public float AngularAcceleration;
    public float MaxSteeringAngle;
    public float MaxBodyInclination;
    public Transform BodyPivot;
    public Text Velocity;

    [SerializeField]
    Rigidbody _rigidbody;
    [SerializeField]
    float _topVelocity;

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
    }

    void Update()
    {
        Velocity.text = string.Format("Velocity: {0:0}", _rigidbody.velocity.magnitude);
        _topVelocity = UnitConverter.KmhToVelocity(TopSpeed);
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

        _rigidbody.AddForce(throttle * transform.forward * Acceleration);

        if (Vector3.Dot(transform.forward, _rigidbody.velocity) < 0)
            _rigidbody.AddTorque(-steering * transform.up * AngularAcceleration);
        else
            _rigidbody.AddTorque(steering * transform.up * AngularAcceleration);
       

        _rigidbody.angularVelocity = Vector3.ClampMagnitude(_rigidbody.angularVelocity, 2f);
        _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, _topVelocity);

        foreach (var axle in Axles)
        {
            if (axle.Steering)
            {
                axle.LeftWheel.localRotation = Quaternion.Lerp(axle.LeftWheel.localRotation, Quaternion.Euler(steering * MaxSteeringAngle, 0, 0), Time.fixedDeltaTime * 5f);
                axle.RightWheel.localRotation = Quaternion.Lerp(axle.RightWheel.localRotation, Quaternion.Euler(steering * MaxSteeringAngle, 0, 0), Time.fixedDeltaTime * 5f);
            }
        }

        BodyPivot.localRotation = Quaternion.Lerp(BodyPivot.localRotation, Quaternion.Euler(MaxBodyInclination * -throttle, BodyPivot.localRotation.y, MaxBodyInclination * steering), Time.fixedDeltaTime * 5f);
    }
}

[System.Serializable]
public class AxleInfo
{
    public Transform LeftWheel;
    public Transform RightWheel;
    public bool Steering;
}