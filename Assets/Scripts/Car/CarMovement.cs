using UnityEngine;
using Assets.Scripts.Car;
using System.Collections.Generic;

[RequireComponent(typeof(Car))]
public class CarMovement : MonoBehaviour
{
    public const int FRONT_WHEELS = 0;
    public const int REAR_WHEELS = 1;

    public List<AxleInfo> Axles;
    public float LinearDrag, AngularDrag;
    public float LinearDragOnAir, AngularDragOnAir;
    public float Acceleration, BrakingPower, AngularAcceleration;
    public float TurnThresholdKMH;
    public float TopSpeedKMH, CurrentTopSpeedKMH, TopSpeedReverseKMH;
    public float MaxSteeringAngle, MaxBodySideAngle, MaxBodyAccelAngle, MaxBodyBrakeAngle;
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
    bool _braking;

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
        _rigidbody = transform.GetComponent<Rigidbody>();

        _trackCount = 0;
        Stopped = true;
        CurrentTopSpeedKMH = TopSpeedKMH;
    }

    void Update()
    {
        _topVelocity = UnitConverter.KmhToVelocity(CurrentTopSpeedKMH);
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

        Move(vInput, vInput, handbrake, hInput);
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
        if (!(InTrack && CanMove && throttle != 0))
            return;

        var forceVector = Vector3.one;

        if (throttle >= 0)
        {
            if (Stopped || MovingForward)
            {
                MovingForward = true;
                forceVector = Acceleration * transform.forward;
            }
            else
            {
                forceVector = BrakingPower * Velocity.normalized;
            }
        }
        else
        {
            if (Stopped || MovingBackwards)
            {
                MovingBackwards = true;
                forceVector = Acceleration * transform.forward;
            }
            else
            {
                forceVector = BrakingPower * Velocity.normalized;
            }
        }

        _rigidbody.AddForce(forceVector * throttle, ForceMode.Acceleration);

        Velocity = Vector3.ClampMagnitude(Velocity, MovingForward ? _topVelocity : _topVelocityReverse);
        Stopped = Speed < 1;
    }

    void ApplySteering(float steering)
    {
        if (!(InTrack && CanMove))
            return;

        var forceVector = (MovingForward ? 1 : -1) * AngularAcceleration * transform.up;

        _rigidbody.AddTorque(forceVector * steering, ForceMode.Acceleration);

        AngularVelocity = Vector3.ClampMagnitude(AngularVelocity, Mathf.Clamp01(Speed * _turnThresholdVelocityMult));
    }

    void UpdateWheels(float steering)
    {
        foreach (var axle in Axles)
        {
            if (axle.Steering)
            {
                // Steering
                var localRot = axle.LeftWheel.localRotation.eulerAngles;
                localRot.y = steering * MaxSteeringAngle;
                axle.LeftWheel.localRotation = Quaternion.Lerp(axle.LeftWheel.localRotation, Quaternion.Euler(localRot), Time.fixedDeltaTime * 5f);
                localRot = axle.RightWheel.localRotation.eulerAngles;
                localRot.y = steering * MaxSteeringAngle;
                axle.RightWheel.localRotation = Quaternion.Lerp(axle.RightWheel.localRotation, Quaternion.Euler(localRot), Time.fixedDeltaTime * 5f);
            }

            //// Motor movement
            //var axleRot = axle.LeftWheel.localRotation.eulerAngles;
            //axleRot.x += SpeedKMH * 25 * (MovingForward ? 1 : -1) * Time.fixedDeltaTime;
            //if (axleRot.x > 90)
            //    axleRot.x = axleRot.x % 90;
            //else if (axleRot.x < 0)
            //    axleRot.x = (90 + axleRot.x) % 90;
            //axleRot.x = Mathf.Clamp(axleRot.x, 0, 90);
            //axle.LeftWheel.localRotation = Quaternion.Euler(axleRot);

            //axleRot = axle.RightWheel.localRotation.eulerAngles;
            //axleRot.x += SpeedKMH * 25 * (MovingForward ? 1 : -1) * Time.fixedDeltaTime;
            //if (axleRot.x > 90)
            //    axleRot.x = axleRot.x % 90;
            //else if (axleRot.x < 0)
            //    axleRot.x = (90 + axleRot.x) % 90;
            //axleRot.x = Mathf.Clamp(axleRot.x, 0, 90);
            //axle.RightWheel.localRotation = Quaternion.Euler(axleRot);
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

    void AddTorque(float Torque, Vector3 axis)
    {
        _rigidbody.AddTorque(Torque * axis);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Track")
        {
            _trackCount++;
            if (InTrack)
            {
                _rigidbody.drag = LinearDrag;
                _rigidbody.angularDrag = AngularDrag;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Track")
        {
            _trackCount--;
            if (!InTrack)
            {
                _rigidbody.drag = LinearDragOnAir;
                _rigidbody.angularDrag = AngularDragOnAir;
            }
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