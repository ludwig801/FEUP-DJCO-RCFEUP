using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CarMovement : MonoBehaviour
{
    public const float VelocityToKMH = 3.6f;
    public const float KMHToVelocity = 1f / 3.6f;

    public List<AxleInfo> Axles;
    public float MaxMotorTorque;        // em que unidade ? 
    public float TopSpeed;              // em que unidade ? 
    public float MaxReverseTorque;      // idem
    public float BrakeTorque;
    public float MaxHandbrakeTorque;
    public float MaxSteeringAngle;
    public float Downforce;
    [Range(0, 1)]
    public float SteeringHelp;
    public bool TractionControl;
    public float TCSlipLimit;
    public Transform CenterOfMass;

    [SerializeField]
    Rigidbody _rigidbody;
    [SerializeField]
    float _currentMotorTorque;
    [SerializeField]
    float[] _forwardSlips;
    float _oldSteerHelpRotation;

    int NumMotorWheels
    {
        get
        {
            return Axles.Where(m => m.Motor).Count() * 2;
        }
    }

    float SpeedKMH
    {
        get
        {
            return _rigidbody.velocity.magnitude * VelocityToKMH;
        }
    }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _currentMotorTorque = TractionControl ? 0 : MaxMotorTorque;

        _forwardSlips = new float[4];

        MaxHandbrakeTorque = float.MaxValue;
    }

    void FixedUpdate()
    {
        var vInput = Input.GetAxis("Vertical");
        var handbrake = Input.GetAxis("Jump");
        var hInput = Input.GetAxis("Horizontal");

        Move(vInput, vInput, handbrake, hInput);
    }

    public void Move(float accelerator, float footbrake, float handbrake, float steering)
    {
        accelerator = Mathf.Clamp(accelerator, 0, 1);
        footbrake = -1 * Mathf.Clamp(footbrake, -1, 0);
        handbrake = Mathf.Clamp(handbrake, 0, 1);
        steering = Mathf.Clamp(steering, -1, 1);

        foreach (AxleInfo axle in Axles)
        {
            ApplySteering(axle, steering);
            ApplyDrive(axle, accelerator, footbrake);
        }

        ClampSpeed(accelerator, footbrake);
        ApplyDownForce();
        ApplyTractionControl();
        ApplySteeringHelp();
    }

    private void ApplySteering(AxleInfo axle, float steering)
    {
        var steerAngle = steering * MaxSteeringAngle;

        if (axle.Steering)
        {
            axle.LeftWheel.steerAngle = steerAngle;
            axle.RightWheel.steerAngle = steerAngle;
        }
    }

    private void ApplySteeringHelp()
    {
        foreach (var axle in Axles)
        {
            WheelHit hit;
            if (!axle.LeftWheel.GetGroundHit(out hit))
                return;
        }


        if (Mathf.Abs(_oldSteerHelpRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - _oldSteerHelpRotation) * SteeringHelp;
            var velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            _rigidbody.velocity = velRotation * _rigidbody.velocity;
        }
        _oldSteerHelpRotation = transform.eulerAngles.y;
    }

    private void ApplyDrive(AxleInfo axle, float accelerator, float footbrake)
    {
        if (axle.Motor)
        {
            var motorTorque = accelerator * (_currentMotorTorque / NumMotorWheels);
            axle.LeftWheel.motorTorque = motorTorque;
            axle.RightWheel.motorTorque = motorTorque;
        }

        if (SpeedKMH > 1 && Vector3.Angle(transform.forward, _rigidbody.velocity) < 50)
        {
            var brakeTorque = footbrake * BrakeTorque;
            axle.LeftWheel.brakeTorque = brakeTorque;
            axle.RightWheel.brakeTorque = brakeTorque;
        }
        else if (footbrake != 0)
        {
            var reverseTorque = footbrake * MaxReverseTorque;
            axle.LeftWheel.motorTorque = -reverseTorque;
            axle.RightWheel.motorTorque = -reverseTorque;
            axle.LeftWheel.brakeTorque = 0;
            axle.RightWheel.brakeTorque = 0;
        }
    }

    private void ClampSpeed(float accelerator, float footbrake)
    {
        if (SpeedKMH > TopSpeed)
        {
            _rigidbody.velocity = (TopSpeed * KMHToVelocity) * _rigidbody.velocity.normalized;
        }

        if (accelerator == 0 && footbrake == 0 && SpeedKMH > -5 && SpeedKMH < 5)
        {
            _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, new Vector3(0, _rigidbody.velocity.y, 0), Time.fixedDeltaTime * 8f);
        }
    }

    private void ApplyDownForce()
    {
        _rigidbody.AddForce(-transform.up * Downforce * _rigidbody.velocity.magnitude);
    }

    private void ApplyTractionControl()
    {
        if (TractionControl)
        {
            var i = 0;
            foreach (var axle in Axles)
            {
                WheelHit hit;
                if (axle.LeftWheel.GetGroundHit(out hit))
                {
                    _forwardSlips[i] = hit.forwardSlip;
                    AdjustTorque(hit.forwardSlip);
                }

                i++;

                if (axle.RightWheel.GetGroundHit(out hit))
                {
                    _forwardSlips[i] = hit.forwardSlip;
                    AdjustTorque(hit.forwardSlip);
                }

                i++;
            }
        }
        else
        {
            _currentMotorTorque = MaxMotorTorque;
        }
    }

    private void AdjustTorque(float forwardSlip)
    {
        if (forwardSlip >= TCSlipLimit && _currentMotorTorque > 0)
        {
            _currentMotorTorque -= 10;
        }
        else
        {
            _currentMotorTorque += 10;
        }

        _currentMotorTorque = Mathf.Clamp(_currentMotorTorque, 0, MaxMotorTorque);
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