using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public Rigidbody Rigidbody;
    public Transform CenterOfMass, MotorForcePosition;
    public float TopSpeedKMH, TopSpeedReverseKMH;
    public float TopAngularSpeedKMH;
    [Range(0, 2)]
    public float TractionControl;
    public Axle[] Axles;
    public TorqueSystem TorqueSystem;
    public Drag LinearDrag, AngularDrag;
    public VehicleState State;

    private void FixedUpdate()
    {
        Rigidbody.centerOfMass = CenterOfMass.localPosition;

        var throttle = Mathf.Clamp(Input.GetAxis("Vertical"), -1, 1);
        var steering = Mathf.Clamp(Input.GetAxis("Horizontal"), -1, 1);

        UpdateCarState();

        ApplyMotor(throttle);
        ApplyTorque(steering);

        ApplyLinearDrag(throttle, steering);
        ApplyAngularDrag(throttle, steering);

        //ApplyTractionControl();

        EvaluateAndClampMovement();
    }

    private void ApplyMotor(float throttle)
    {
        if (!State.CanMove || !State.Grounded || !TorqueSystem.UseMotorTorque || throttle == 0)
            return;

        State.MovingForward = (throttle > 0 && !State.MovingReverse);
        State.MovingReverse = (throttle < 0 && !State.MovingForward);

        var forceVector = TorqueSystem.MotorTorque * State.GroundForward;

        if (State.MovingForward && throttle < 0 || State.MovingReverse && throttle < 0)
            forceVector *= TorqueSystem.BrakeFactor;

        Rigidbody.AddForceAtPosition(forceVector * Rigidbody.mass * throttle, MotorForcePosition.position, ForceMode.Force);
    }

    private void ApplyTorque(float steering)
    {
        if (!State.CanMove || !State.Grounded || !State.GroundedOnSteering || !TorqueSystem.UseAngularTorque || steering == 0)
            return;

        if (State.MovingReverse)
            steering = -steering;

        Rigidbody.AddTorque(steering * Rigidbody.mass * TorqueSystem.AngularTorque * transform.up, ForceMode.Force);
    }

    private void ApplyLinearDrag(float throttle, float steering)
    {
        if (!State.CanMove || !State.Grounded)
            Rigidbody.drag = 0;
        else if (steering != 0 || throttle != 0)
            Rigidbody.drag = LinearDrag.MinValue;
        else if (throttle == 0 && steering == 0)
            Rigidbody.drag = Mathf.Clamp(Rigidbody.drag + LinearDrag.Increment, LinearDrag.MinValue, LinearDrag.MaxValue);

    }

    private void ApplyAngularDrag(float throttle, float steering)
    {
        if (!State.CanMove || !State.Grounded)
            Rigidbody.angularDrag = 0;
        else if (steering != 0 || throttle != 0)
            Rigidbody.angularDrag = AngularDrag.MinValue;
        else if (steering == 0)
            Rigidbody.angularVelocity = Vector3.Lerp(Rigidbody.angularVelocity, Vector3.zero, Time.fixedDeltaTime * 2.5f);
    }

    private void ApplyTractionControl()
    {
        if (!State.CanMove || !State.Grounded || State.Stopped || TractionControl <= 0)
            return;

        var forwardComponent = Vector3.Project(Rigidbody.velocity, State.GroundForward);
        var sideComponent = Vector3.Project(Rigidbody.velocity, Rigidbody.transform.right);
        var clampedSideComponent = Vector3.ClampMagnitude(sideComponent, TractionControl * sideComponent.magnitude);
        //Rigidbody.velocity = forwardComponent + sideComponent;
        Debug.DrawRay(Rigidbody.transform.position, sideComponent, Color.white);
        Debug.DrawRay(Rigidbody.transform.position, -clampedSideComponent, Color.blue);
        Rigidbody.AddForce(-0.25f * clampedSideComponent, ForceMode.VelocityChange);
        sideComponent = Vector3.Project(Rigidbody.velocity, Rigidbody.transform.right);
        Debug.DrawRay(Rigidbody.transform.position, sideComponent, Color.yellow);
    }

    private void EvaluateAndClampMovement()
    {
        if (State.MovingForward)
            Rigidbody.velocity = Vector3.ClampMagnitude(Rigidbody.velocity, UnitConverter.KmhToVelocity(TopSpeedKMH));
        else if (State.MovingReverse)
            Rigidbody.velocity = Vector3.ClampMagnitude(Rigidbody.velocity, UnitConverter.KmhToVelocity(TopSpeedReverseKMH));

        if (State.Stopped)
            Rigidbody.angularVelocity = Vector3.zero;
        else
            Rigidbody.angularVelocity = Vector3.ClampMagnitude(Rigidbody.angularVelocity, UnitConverter.KmhToVelocity(TopAngularSpeedKMH) / Mathf.PI);

        State.Stopped = Rigidbody.velocity.magnitude < 1;
        //if (State.Stopped && State.CanMove)
        //    Rigidbody.velocity = Vector3.Lerp(Rigidbody.velocity, Vector3.zero, Time.fixedDeltaTime);
    }

    private void UpdateCarState()
    {
        State.Grounded = false;
        State.GroundedOnSteering = false;

        var groundNormal = Vector3.zero;
        var groundedCount = 0;

        foreach (var axle in Axles)
        {
            if (axle.LeftWheel.Grounded || axle.RightWheel.Grounded)
            {
                if (axle.LeftWheel.Grounded)
                {
                    groundNormal += axle.LeftWheel.Normal;
                    groundedCount++;
                }

                if (axle.RightWheel.Grounded)
                {
                    groundNormal += axle.LeftWheel.Normal;
                    groundedCount++;
                }

                State.Grounded = true;
                if (axle.Steering)
                    State.GroundedOnSteering = true;
            }
        }

        groundNormal /= groundedCount;
        State.GroundForward = Vector3.Cross(Rigidbody.transform.right, groundNormal);
    }
}

[System.Serializable]
public class Axle
{
    public CarWheel LeftWheel;
    public CarWheel RightWheel;
    public bool Steering;
}

[System.Serializable]
public class Drag
{
    [Range(0, 10)]
    public float MinValue;

    [Range(0, 10)]
    public float MaxValue;

    [Range(0, 1)]
    public float Increment;
}

[System.Serializable]
public class TorqueSystem
{
    public float MotorTorque;
    public float BrakeFactor;
    public float AngularTorque;
    public bool UseMotorTorque;
    public bool UseAngularTorque;
}

[System.Serializable]
public class VehicleState
{
    public Vector3 GroundForward;
    public bool CanMove, Grounded, GroundedOnSteering;
    [SerializeField]
    private bool _movingForward, _movingReverse, _stopped;

    public bool MovingForward
    {
        get
        {
            return _movingForward;
        }

        set
        {
            _movingForward = value;
            _movingReverse = _movingReverse && !value;
            _stopped = _stopped && !value;
        }
    }

    public bool MovingReverse
    {
        get
        {
            return _movingReverse;
        }

        set
        {
            _movingReverse = value;
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
            _movingReverse = _movingReverse && !value;
            _movingForward = _movingForward && !value;
        }
    }
}