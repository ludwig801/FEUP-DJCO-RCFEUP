using UnityEngine;

public class CarMovement : MonoBehaviour
{
	public Rigidbody Rigidbody;
	public Transform CenterOfMass, MotorForcePosition;
    [Range(0, 1)]
    public float TractionControl;
    public SpeedLimits SpeedStatsKMH;
	public TorqueSystem TorqueSystem;
	public VehicleState State;
    public VisualElements Visuals;
    public AudioSource CrashAudio;

    void Start()
    {
        TorqueSystem.CurrentMotorTorque = TorqueSystem.MotorTorque;
        TorqueSystem.CurrentAngularTorque = TorqueSystem.AngularTorque;

        SpeedStatsKMH.CurrentTopSpeed = SpeedStatsKMH.TopSpeed;
        SpeedStatsKMH.CurrentTopSpeedReverse = SpeedStatsKMH.TopSpeedReverse;
        SpeedStatsKMH.CurrentTopTurningSpeed = SpeedStatsKMH.TopTurningSpeed;
    }

    void Update()
    {
        ApplySideTilt();
        ApplyThrottleTilt();
        ApplyWheelsSteering();
    }

	void FixedUpdate()
	{
		Rigidbody.centerOfMass = CenterOfMass.localPosition;

		State.CurrentThrottle = Mathf.Clamp(Input.GetAxis("Vertical"), -1, 1);
        State.CurrentSteering = Mathf.Clamp(Input.GetAxis("Horizontal"), -1, 1);

		ApplyMotor();
		ApplyTorque();

		ApplyTractionControl();
		EvaluateAndClampMovement();

		Debug.DrawRay(Rigidbody.transform.position, Rigidbody.velocity, Color.green);
	}

	private void ApplyMotor()
	{
        var throttle = State.CurrentThrottle;

		if (!State.CanMove || !State.Grounded || !TorqueSystem.UseMotorTorque || throttle == 0)
			return;

		if (throttle > 0)
			State.MovingForward = State.MovingForward || State.Stopped || Vector3.Dot(Rigidbody.velocity, transform.forward) >= 0;
		else if(throttle < 0)
			State.MovingReverse = State.MovingReverse || State.Stopped || Vector3.Dot(Rigidbody.velocity, transform.forward) <= 0;

		Rigidbody.AddForceAtPosition(Rigidbody.transform.forward * TorqueSystem.CurrentMotorTorque * throttle, MotorForcePosition.position, ForceMode.Acceleration);
	}

	private void ApplyTorque()
	{
        var steering = State.CurrentSteering;

		if (!State.CanMove || !State.Grounded || !State.GroundedOnSteering || !TorqueSystem.UseAngularTorque || steering == 0)
			return;

		if (State.MovingReverse)
			steering = -steering;

		Rigidbody.AddTorque(Rigidbody.transform.up * steering * TorqueSystem.CurrentAngularTorque, ForceMode.VelocityChange);

        State.CurrentSteering = steering;
	}

	private void ApplyTractionControl()
	{
		if (!State.CanMove || !State.Grounded || State.Stopped)
			return;

		//var forwardComponent = Vector3.Project(Rigidbody.velocity, State.GroundForward);
		var sideComponent = Vector3.Project(Rigidbody.velocity, Rigidbody.transform.right);
		var clampedSideComponent = sideComponent * TractionControl;

		Rigidbody.AddForce(-clampedSideComponent * 10, ForceMode.Acceleration);

		Debug.DrawRay(Rigidbody.transform.position, sideComponent, Color.blue);
		Debug.DrawRay(Rigidbody.transform.position, -clampedSideComponent, Color.red);
	}

	private void EvaluateAndClampMovement()
	{
		if (State.MovingForward)
			Rigidbody.velocity = Vector3.ClampMagnitude(Rigidbody.velocity, UnitConverter.KmhToVelocity(SpeedStatsKMH.CurrentTopSpeed));
		else if (State.MovingReverse)
			Rigidbody.velocity = Vector3.ClampMagnitude(Rigidbody.velocity, UnitConverter.KmhToVelocity(SpeedStatsKMH.CurrentTopSpeedReverse));

		if (State.Stopped)
			Rigidbody.angularVelocity = Vector3.zero;
		else
			Rigidbody.angularVelocity = Vector3.ClampMagnitude(Rigidbody.angularVelocity, UnitConverter.KmhToVelocity(SpeedStatsKMH.CurrentTopTurningSpeed) / Mathf.PI);

		State.Stopped = Rigidbody.velocity.magnitude < 5;
	}

	private void OnCollisionEnter(Collision collision)
	{
        if (collision.gameObject.tag == "Track")
        {
            State.Grounded = true;
            State.GroundedOnSteering = true;
        }
        else if (collision.gameObject.tag == "Track Walls")
        {
            if(!CrashAudio.isPlaying)
                CrashAudio.Play();
        }
	}

	private void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject.tag == "Track")
		{
			State.Grounded = true;
			State.GroundedOnSteering = true;
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.tag == "Track")
		{
			State.Grounded = false;
			State.GroundedOnSteering = false;
		}
	}

    private void ApplySideTilt()
    {
        if (!Visuals.SideTilt)
            return;

        var currentEuler = Visuals.CarPivot.localEulerAngles;
        Visuals.CarPivot.localRotation = Quaternion.Lerp(Visuals.CarPivot.localRotation, Quaternion.Euler(currentEuler.x, currentEuler.y, State.CurrentSteering * Visuals.MaxSideTilt), Time.deltaTime * Visuals.TiltSpeed);
    }

    private void ApplyThrottleTilt()
    {
        if (!Visuals.ThrottleTilt)
            return;

        var currentEuler = Visuals.CarPivot.localEulerAngles;
        Visuals.CarPivot.localRotation = Quaternion.Lerp(Visuals.CarPivot.localRotation, Quaternion.Euler(-State.CurrentThrottle * Visuals.MaxThrottleTilt, currentEuler.y, currentEuler.z), Time.deltaTime * Visuals.TiltSpeed); 
    }

    private void ApplyWheelsSteering()
    {
        if (!Visuals.Steering)
            return;

        if (Visuals.FrontAxle.SteerAxle)
        {
            var leftWheel = Visuals.FrontAxle.LeftWheel;
            leftWheel.localRotation = Quaternion.Lerp(leftWheel.localRotation, Quaternion.Euler(leftWheel.localRotation.x, State.CurrentSteering * Visuals.MaxSteering, leftWheel.localRotation.z), Time.deltaTime * Visuals.SteeringSpeed);
            Visuals.FrontAxle.RightWheel.localRotation = leftWheel.localRotation;
        }

        if (Visuals.RearAxle.SteerAxle)
        {
            var leftWheel = Visuals.RearAxle.LeftWheel;
            leftWheel.localRotation = Quaternion.Lerp(leftWheel.localRotation, Quaternion.Euler(leftWheel.localRotation.x, State.CurrentSteering * Visuals.MaxSteering, leftWheel.localRotation.z), Time.deltaTime * Visuals.SteeringSpeed);
            Visuals.RearAxle.RightWheel.localRotation = leftWheel.localRotation;
        }
    }

    public void Reset()
    {
        Rigidbody.velocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
        State.CanMove = false;
    }
}

[System.Serializable]
public class Axle
{
	public CarWheel LeftWheel, RightWheel;
}

[System.Serializable]
public class TorqueSystem
{
	public float MotorTorque;
	public float BrakeFactor;
	public float AngularTorque;
	public bool UseMotorTorque;
	public bool UseAngularTorque;
    public float CurrentMotorTorque;
    public float CurrentAngularTorque;
}

[System.Serializable]
public class SpeedLimits
{
    public float TopSpeed, TopSpeedReverse;
    public float TopTurningSpeed;
    public float CurrentTopSpeed, CurrentTopSpeedReverse;
    public float CurrentTopTurningSpeed;
}

[System.Serializable]
public class VehicleState
{
	public Vector3 GroundForward;
	public bool CanMove, Grounded, GroundedOnSteering;
    public float CurrentThrottle, CurrentSteering;

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

[System.Serializable]
public class VisualAxle
{
    public Transform LeftWheel, RightWheel;
    public bool SteerAxle;
}

[System.Serializable]
public class VisualElements
{
    public Transform CarPivot;
    public float TiltSpeed;
    public bool SideTilt;
    [Range(0, 45)]
    public int MaxSideTilt;
    public bool ThrottleTilt;
    [Range(0, 45)]
    public int MaxThrottleTilt;

    public bool Steering;
    public float SteeringSpeed;
    public VisualAxle FrontAxle, RearAxle;
    [Range(0, 60)]
    public int MaxSteering;
}