using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public RaceManager RaceManager;
    public Rigidbody Rigidbody;
	public Transform CenterOfMass, MotorForcePosition;
    public float Downforce;
    [Range(0, 1)]
    public float TractionControl;
    public SpeedLimits SpeedStatsKMH;
	public TorqueSystem TorqueSystem;
	public VehicleState State;
    public VisualElements Visuals;
    public VehicleAudio Audio;
    public VehicleInput CarInput;

    public int WallHitCount;

    void Start()
    {
        RaceManager = RaceManager.Instance;

        TorqueSystem.CurrentMotorTorque = TorqueSystem.MotorTorque;
        TorqueSystem.CurrentAngularTorque = TorqueSystem.AngularTorque;

        SpeedStatsKMH.CurrentTopSpeed = SpeedStatsKMH.TopSpeed;
        SpeedStatsKMH.CurrentTopSpeedReverse = SpeedStatsKMH.TopSpeedReverse;
        SpeedStatsKMH.CurrentTopTurningSpeed = SpeedStatsKMH.TopTurningSpeed;

        Audio.AccelerationAudio.Play();
        Audio.SetBounds(0, SpeedStatsKMH.TopSpeed);

        WallHitCount = 0;
    }

    void Update()
    {
        ApplySideTilt();
        ApplyThrottleTilt();
        ApplyWheelsSteering();
        ApplySound();
    }

	void FixedUpdate()
	{
		Rigidbody.centerOfMass = CenterOfMass.localPosition;

        CarInput.CollectInput();

		State.CurrentThrottle = CarInput.Throttle;
        State.CurrentSteering = CarInput.Steering;

		ApplyMotor();
		ApplyTorque();

		ApplyTractionControl();
		EvaluateAndClampMovement();

        ApplyDownforce();
	}

	private void ApplyMotor()
	{
        var throttle = State.CurrentThrottle;

		if (!State.CanMove || !State.Grounded || !TorqueSystem.UseMotorTorque || throttle == 0)
			return;

        if (throttle > 0)
            State.MovingForward = State.MovingForward || State.Stopped || Vector3.Dot(Rigidbody.velocity, transform.forward) >= 0;
        else if (throttle < 0)
            State.MovingReverse = State.MovingReverse || State.Stopped || Vector3.Dot(Rigidbody.velocity, transform.forward) <= 0;

        var motorForce = TorqueSystem.CurrentMotorTorque;
        if ((throttle > 0 && State.MovingReverse) || (throttle < 0 && State.MovingForward))
            motorForce *= TorqueSystem.BrakeFactor;

        Rigidbody.AddForceAtPosition(Rigidbody.transform.forward * motorForce * throttle, MotorForcePosition.position, ForceMode.Acceleration);
	}

	private void ApplyTorque()
	{
        var steering = State.CurrentSteering;

		if (!State.CanMove || !State.Grounded || !State.GroundedOnSteering || !TorqueSystem.UseAngularTorque || steering == 0)
			return;

		if (State.MovingReverse)
			steering = -steering;

		Rigidbody.AddTorque(Vector3.up * steering * TorqueSystem.CurrentAngularTorque, ForceMode.VelocityChange);

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

    private void ApplyDownforce()
    {
        var axis = Vector3.Cross(Rigidbody.transform.up, Vector3.up);
        var multiplier = (Vector3.up - Rigidbody.transform.up).magnitude;

        Rigidbody.AddTorque(axis * multiplier * Downforce, ForceMode.Acceleration);

        if (State.Grounded)
            return;

        var ray = new Ray(Rigidbody.transform.position, Vector3.down);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 50))
        {
            if (hitInfo.collider.tag == "Track")
                Rigidbody.AddForce(Vector3.down * Downforce * hitInfo.distance * 10, ForceMode.Acceleration);
        }
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
            WallHitCount++;

            if (!Audio.CrashAudio.isPlaying)
                Audio.CrashAudio.Play();
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

    private void ApplySound()
    {
        var accelAudio = Audio.AccelerationAudio;
        if (RaceManager.State.Paused)
            accelAudio.volume = Mathf.Lerp(accelAudio.volume, 0, Time.unscaledDeltaTime * Audio.LerpSpeed);
        else
        {
            accelAudio.volume = Audio.Volume;
            accelAudio.pitch = Mathf.Lerp(accelAudio.pitch, UnitConverter.VelocityToKMH(Rigidbody.velocity.magnitude) * Audio.InvertedDelta + 0.5f, Time.deltaTime * Audio.LerpSpeed);
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

[System.Serializable]
public class VehicleAudio
{
    public AudioSource AccelerationAudio, CrashAudio;
    public float MinAccelBoundKMH, MaxAccelBoundKMH;
    public float Delta, InvertedDelta;
    public float LerpSpeed;
    public float Volume;

    public void SetBounds(float min, float max)
    {
        MinAccelBoundKMH = Mathf.Max(0, min);
        MaxAccelBoundKMH = Mathf.Max(max, MinAccelBoundKMH);
        Delta = MaxAccelBoundKMH - MinAccelBoundKMH;
        InvertedDelta = 1f / Delta;
    }
}

[System.Serializable]
public class VehicleInput
{
    public int Index;
    public float Throttle;
    public float Steering;

    public void CollectInput()
    {
        switch (Index)
        {
            case 0:
                Throttle = Input.GetAxis("Vertical 0");
                Steering = Input.GetAxis("Horizontal 0");
                break;

            case 1:
                Throttle = Input.GetAxis("Vertical 1");
                Steering = Input.GetAxis("Horizontal 1");
                break;
        }
    }
}