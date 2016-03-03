using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Car;
using System.Collections.Generic;

public class CarMovement : MonoBehaviour
{
    public const int FRONT_WHEELS = 0;
    public const int REAR_WHEELS = 1;

    public List<AxleInfo> Axles;
    public float Acceleration;
    public float ReverseAcceleration;
    public float TopSpeed;
    public float AngularAcceleration;
    public float HandbrakeStrength;
    public float MaxSteeringAngle;
    public float TurnThreshold;
    public float MaxBodySideAngle;
    public float MaxBodyAccelAngle;
    public float MaxBodyBrakeAngle;
    public Transform BodyPivot, CenterOfMass;
    public Text VelocityText;

    [SerializeField]
    Rigidbody _rigidbody;
    [SerializeField]
    float _topVelocity;
    [SerializeField]
    bool _movingForward;
    [SerializeField]
    int track;

    public bool Flying
    {
        get
        {
            return track == 0;
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

        track = 0;
    }

    void Update()
    {
        if (VelocityText != null)
        {
            VelocityText.text = string.Format("Velocity: {0:0}", SpeedKMH);
        }
        _topVelocity = UnitConverter.KmhToVelocity(TopSpeed);
    }

    void FixedUpdate()
    {
        _rigidbody.centerOfMass = CenterOfMass.localPosition;

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

        _movingForward = Vector3.Dot(transform.forward, Velocity) >= 0;

        ApplyDrive(throttle, handbrake);
        ApplySteering(steering);

        UpdateWheels(steering);
        UpdateBody(throttle, steering);

        ClampRotation();
    }

    void ApplyDrive(float throttle, float handbrake)
    {
        if (!Flying)
        {
            var realForward = new Vector3(transform.forward.x, 0, transform.forward.z);
            //var realForward = transform.forward;
            var vectorReference = throttle >= 0 ?
                (_movingForward ? realForward : -Velocity.normalized) :
                (_movingForward ? Velocity.normalized : realForward);
            var accel = throttle > 0 ? Acceleration : ReverseAcceleration;

            AddForce(throttle * accel * Mass, vectorReference);
        }

        Velocity = Vector3.ClampMagnitude(Velocity, _topVelocity);
    }

    void ApplySteering(float steering)
    {
        if (!Flying)
        {
            AddTorque((_movingForward ? 1 : -1) * steering * AngularAcceleration * Mass, transform.up);

            var turnClamp = Mathf.Clamp01(Speed / TurnThreshold);
            AngularVelocity = Vector3.ClampMagnitude(AngularVelocity, turnClamp);
        }
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
        if (Flying)
        {
            //AddTorque(800 * Mass, -Vector3.Cross(collision.contacts[0].normal, transform.up));
            //AddForce(500 * Mass, -transform.up);

            //    //var rotationX = transform.rotation.x;
            //    //if (rotationX < 45 || rotationX > 315)
            //    //{
            //    //    AddForce(250 * Mass, -transform.up);
            //    //}
            //    //else if (rotationX > 180)
            //    //{
            //    //    var axle = Axles[FRONT_WHEELS];
            //    //    AddForce(400 * Mass, -transform.up, Vector3.Lerp(axle.LeftWheel.transform.position, axle.RightWheel.transform.position, 0.5f));
            //    //}
            //    //else
            //    //{
            //    //    var axle = Axles[REAR_WHEELS];
            //    //    AddForce(400 * Mass, -transform.up, Vector3.Lerp(axle.LeftWheel.transform.position, axle.RightWheel.transform.position, 0.5f));
            //    //}
        }

        if (collision.gameObject.tag == "Track")
            track++;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Track")
            track--;
    }
}

[System.Serializable]
public class AxleInfo
{
    public Transform LeftWheel;
    public Transform RightWheel;
    public bool Steering;
    //public float Pressure;
    //public float SpringMaxDistance;
    //public float Center;
}