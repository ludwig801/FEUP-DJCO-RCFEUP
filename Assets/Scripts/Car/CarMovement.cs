using UnityEngine;

public class CarMovement : MonoBehaviour
{
	public Rigidbody Rigidbody;
	public Transform CenterOfMass, MotorForcePosition;
	public float TopSpeedKMH, TopSpeedReverseKMH;
	public float TopAngularSpeedKMH;
	[Range(0, 1)]
	public float TractionControl;
	public TorqueSystem TorqueSystem;
	public VehicleState State;

	private void FixedUpdate()
	{
		Rigidbody.centerOfMass = CenterOfMass.localPosition;

		var throttle = Mathf.Clamp(Input.GetAxis("Vertical"), -1, 1);
		var steering = Mathf.Clamp(Input.GetAxis("Horizontal"), -1, 1);

		ApplyMotor(throttle);
		ApplyTorque(steering);

		ApplyTractionControl();
		EvaluateAndClampMovement();

		Debug.DrawRay(Rigidbody.transform.position, Rigidbody.velocity, Color.green);
	}

	private void ApplyMotor(float throttle)
	{
		if (!State.CanMove || !State.Grounded || !TorqueSystem.UseMotorTorque || throttle == 0)
			return;

		if (throttle > 0)
			State.MovingForward = State.MovingForward || State.Stopped || Vector3.Dot(Rigidbody.velocity, transform.forward) >= 0;
		else if(throttle < 0)
			State.MovingReverse = State.MovingReverse || State.Stopped || Vector3.Dot(Rigidbody.velocity, transform.forward) <= 0;

		Rigidbody.AddForceAtPosition(Rigidbody.transform.forward * TorqueSystem.MotorTorque * throttle, MotorForcePosition.position, ForceMode.Acceleration);
	}

	private void ApplyTorque(float steering)
	{
		if (!State.CanMove || !State.Grounded || !State.GroundedOnSteering || !TorqueSystem.UseAngularTorque || steering == 0)
			return;

		if (State.MovingReverse)
			steering = -steering;

		Rigidbody.AddTorque(Rigidbody.transform.up * steering * TorqueSystem.AngularTorque, ForceMode.VelocityChange);
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
			Rigidbody.velocity = Vector3.ClampMagnitude(Rigidbody.velocity, UnitConverter.KmhToVelocity(TopSpeedKMH));
		else if (State.MovingReverse)
			Rigidbody.velocity = Vector3.ClampMagnitude(Rigidbody.velocity, UnitConverter.KmhToVelocity(TopSpeedReverseKMH));

		if (State.Stopped)
			Rigidbody.angularVelocity = Vector3.zero;
		else
			Rigidbody.angularVelocity = Vector3.ClampMagnitude(Rigidbody.angularVelocity, UnitConverter.KmhToVelocity(TopAngularSpeedKMH) / Mathf.PI);

		State.Stopped = Rigidbody.velocity.magnitude < 5;
		//if (State.Stopped && State.CanMove)
		//    Rigidbody.velocity = Vector3.Lerp(Rigidbody.velocity, Vector3.zero, Time.fixedDeltaTime);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Track")
		{
			State.Grounded = true;
			State.GroundedOnSteering = true;
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
}

[System.Serializable]
public class Axle
{
	public CarWheel LeftWheel;
	public CarWheel RightWheel;
	public bool Steering;
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