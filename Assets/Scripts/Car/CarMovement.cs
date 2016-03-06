using UnityEngine;
using Assets.Scripts.Car;
using System.Collections.Generic;

[RequireComponent(typeof(Car))]
public class CarMovement : MonoBehaviour
{
    public const int FRONT_WHEELS = 0;
    public const int REAR_WHEELS = 1;

    public List<AxleInfo> Axles;
    [Range(1.5f, 6)]
    public float Acceleration;
    [Range(1.5f, 6)]
    public float BrakingPower;
    [Range(3, 6)]
    public float AngularAcceleration;
    [Range(0, 20)]
    public float TurnThresholdKMH;
    [Range(50, 300)]
    public float TopSpeedKMH;
    [Range(0, 50)]
    public float TopSpeedReverseKMH;
    public float MaxSteeringAngle;
    public float MaxBodySideAngle;
    public float MaxBodyAccelAngle;
    public float MaxBodyBrakeAngle;
    public float MaxInclinationX, MaxInclinationZ;
    public Transform BodyPivot, CenterOfMass;
    public PowerUp PowerUp;
    public bool CanMove;

    [SerializeField]
    Rigidbody _rigidbody;
    [SerializeField]
    float _topVelocity;
    [SerializeField]
    float _topVelocityReverse;
    [SerializeField]
    float _turnThresholdVelocityMult;
    [SerializeField]
    int _trackCount;
    [SerializeField]
    bool _movingForward;
    [SerializeField]
    bool _movingBackwards;
    [SerializeField]
    bool _stopped;
    [SerializeField]
    Vector3 _oldVelocity;

    public bool MovingForward
    {
        get
        {
            return _movingForward;
        }

        set
        {
            _movingForward = value;
            _movingBackwards = _movingBackwards && !value;
            _stopped = _stopped && !value;
        }
    }

    public bool MovingBackwards
    {
        get
        {
            return _movingBackwards;
        }

        set
        {
            _movingBackwards = value;
            _movingForward = _movingForward && !value;
            _stopped = _stopped && !value;
        }
    }

    public bool Stopped
    {
        get
        {
            return _stopped;
        }

        set
        {
            _stopped = value;
            _movingBackwards = _movingBackwards && !value;
            _movingForward = _movingForward && !value;
        }
    }

    public bool InTrack
    {
        get
        {
            return _trackCount > 0;
        }
    }

    public float Mass
    {
        get
        {
            return _rigidbody.mass;
        }

        private set
        {
            _rigidbody.mass = value;
        }
    }

    public Vector3 Velocity
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

    public Vector3 AngularVelocity
    {
        get
        {
            return _rigidbody.angularVelocity;
        }

        set
        {
            _rigidbody.angularVelocity = value;
        }
    }

    public float SpeedKMH
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

        _trackCount = 0;
        Stopped = true;
        _oldVelocity = _rigidbody.velocity;
    }

    void Update()
    {
        _topVelocity = UnitConverter.KmhToVelocity(TopSpeedKMH);
        _topVelocityReverse = UnitConverter.KmhToVelocity(TopSpeedReverseKMH);
        _turnThresholdVelocityMult = 1f / UnitConverter.KmhToVelocity(TurnThresholdKMH);

        if (!CanMove)
            _rigidbody.constraints = (RigidbodyConstraints)10; // Freeze all movement except on Y axis
        else
            _rigidbody.constraints = RigidbodyConstraints.None;
    }

    void FixedUpdate()
    {
        var vInput = Input.GetAxis("Vertical");
        var handbrake = Input.GetAxis("Jump");
        var hInput = Input.GetAxis("Horizontal");

        if (Vector3.Dot(_rigidbody.velocity, _oldVelocity) < 0)
            _movingForward = !_movingForward;

        Move(vInput, vInput, handbrake, hInput);

        _oldVelocity = _rigidbody.velocity;
    }

    public void ReduceMass(float value)
    {
        Mass -= value;
    }

    public void Move(float throttle, float footbrake, float handbrake, float steering)
    {
        throttle = Input.GetAxisRaw("Vertical");
        steering = Input.GetAxisRaw("Horizontal");
        handbrake = Mathf.Clamp(handbrake, 0, 1);

        ApplyDrive(throttle, handbrake);
        ApplySteering(steering);

        UpdateWheels(steering);
        UpdateBody(throttle, steering);

        ClampRotation();
    }

    void ApplyDrive(float throttle, float handbrake)
    {
        if (InTrack && CanMove)
        {
            var accel = 0f;
            var vectorReference = Vector3.zero;
            if (throttle >= 0)
            {
                if (Stopped)
                {
                    MovingForward = true;
                    accel = Acceleration;
                    vectorReference = transform.forward;
                }
                else if (MovingForward)
                {
                    accel = Acceleration;
                    vectorReference = transform.forward;
                }
                else
                {
                    accel = BrakingPower;
                    vectorReference = -Velocity.normalized;
                }
            }
            else
            {
                if (Stopped)
                {
                    MovingBackwards = true;
                    accel = Acceleration;
                    vectorReference = transform.forward;
                }
                else if (MovingBackwards)
                {
                    accel = Acceleration;
                    vectorReference = transform.forward;
                }
                else
                {
                    accel = BrakingPower;
                    vectorReference = Velocity.normalized;
                }
            }

            AddForce(throttle * accel * Mass * 10, vectorReference);
        }

        Velocity = Vector3.ClampMagnitude(Velocity, MovingForward ? _topVelocity : _topVelocityReverse);
        Stopped = Speed < 0.05f;
    }

    void ApplySteering(float steering)
    {
        if (InTrack && CanMove)
        {
            AddTorque((_movingForward ? 1 : -1) * steering * AngularAcceleration * Mass * 10, transform.up);
        }

        var turnClamp = Mathf.Clamp01(Speed * _turnThresholdVelocityMult);
        AngularVelocity = Vector3.ClampMagnitude(AngularVelocity, turnClamp);
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
        rotation.x = Utils.ClampRotation(rotation.x, MaxInclinationX);
        rotation.z = Utils.ClampRotation(rotation.z, MaxInclinationZ);
        transform.localRotation = Quaternion.Euler(rotation);
    }

    void AddForce(float force, Vector3 direction)
    {
        _rigidbody.AddForce(force * direction);
    }

    void AddForce(float force, Vector3 direction, Vector3 point)
    {
        _rigidbody.AddForceAtPosition(force * direction, point);
    }

    void AddTorque(float Torque, Vector3 axis)
    {
        _rigidbody.AddTorque(Torque * axis);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Track")
        {
            _trackCount++;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Track")
        {
            _trackCount--;
        }
    }
}

[System.Serializable]
public class AxleInfo
{
    public Transform LeftWheel;
    public Transform RightWheel;
    public bool Steering;
}