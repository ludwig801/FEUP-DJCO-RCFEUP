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
    public float HandbrakeStrength;
    public float MaxSteeringAngle;
    public float TurnThreshold;
    public float MaxBodySideAngle;
    public float MaxBodyAccelAngle;
    public float MaxBodyBrakeAngle;
    public Transform BodyPivot;
    public Text VelocityText;

    [SerializeField]
    Rigidbody _rigidbody;
    [SerializeField]
    float _topVelocity;
    [SerializeField]
    bool _flying;
    [SerializeField]
    bool _movingForward;

    Vector3 Velocity
    {
        get
        {
            return _rigidbody.velocity;
        }

        set
        {
            _rigidbody.velocity = value;
        }
    }

    float SpeedKMH
    {
        get
        {
            return UnitConverter.VelocityToKmh(Velocity.magnitude);
        }
    }

    float Speed
    {
        get
        {
            return Velocity.magnitude;
        }
    }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        VelocityText.text = string.Format("Velocity: {0:0}", Speed);
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

        _movingForward = Vector3.Dot(transform.forward, Velocity) >= 0;

        ApplyDrive(throttle, handbrake);
        ApplySteering(steering);

        UpdateWheels(steering);
        UpdateBody(throttle, steering);

        ClampRotation();
    }

    void ApplyDrive(float throttle, float handbrake)
    {
        if (!_flying)
        {
            var realForward = new Vector3(transform.forward.x, 0, transform.forward.z);
            var vectorReference = throttle >= 0 ?
                (_movingForward ? realForward : -Velocity.normalized) :
                (_movingForward ? Velocity.normalized : realForward);

            _rigidbody.AddForce(throttle * vectorReference * Acceleration * _rigidbody.mass);
        }

        Velocity = Vector3.ClampMagnitude(Velocity, _topVelocity);
    }

    void ApplySteering(float steering)
    {
        _rigidbody.AddTorque((_movingForward ? 1 : -1) * (_flying ? 0.25f : 1) * steering * transform.up * AngularAcceleration * _rigidbody.mass);

        var turnClamp = Mathf.Clamp01(Speed / TurnThreshold);
        _rigidbody.angularVelocity = Vector3.ClampMagnitude(_rigidbody.angularVelocity, turnClamp);
    }

    void UpdateWheels(float steering)
    {
        foreach (var axle in Axles)
        {
            if (axle.Steering)
            {
                axle.LeftWheel.localRotation = Quaternion.Lerp(axle.LeftWheel.localRotation, Quaternion.Euler(steering * MaxSteeringAngle, 0, 0), Time.fixedDeltaTime * 5f);
                axle.RightWheel.localRotation = Quaternion.Lerp(axle.RightWheel.localRotation, Quaternion.Euler(steering * MaxSteeringAngle, 0, 0), Time.fixedDeltaTime * 5f);
            }
        }
    }

    void UpdateBody(float throttle, float steering)
    {
        var sideRotation = MaxBodySideAngle * steering;
        var forwardRotation = throttle >= 0 ? throttle * -MaxBodyAccelAngle : throttle * -MaxBodyBrakeAngle;
        BodyPivot.localRotation = Quaternion.Lerp(BodyPivot.localRotation, Quaternion.Euler(forwardRotation, BodyPivot.localRotation.y, sideRotation), Time.fixedDeltaTime * 5f);
    }

    void ClampRotation()
    {
        var rotation = transform.localRotation.eulerAngles;
        rotation.x = Utils.ClampRotation(rotation.x, 45);
        rotation.z = Utils.ClampRotation(rotation.z, 10);
        transform.localRotation = Quaternion.Euler(rotation);
    }

    void OnCollisionEnter(Collision other)
    {
        _flying = false;
    }

    void OnCollisionStay(Collision other)
    {
        _flying = false;
    }

    void OnCollisionExit(Collision other)
    {
        _flying = true;
    }
}

[System.Serializable]
public class AxleInfo
{
    public Transform LeftWheel;
    public Transform RightWheel;
    public bool Steering;
}