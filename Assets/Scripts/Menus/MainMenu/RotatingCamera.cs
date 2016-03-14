using UnityEngine;
using System.Collections;

public class RotatingCamera : MonoBehaviour {

	public Transform target;
	public float speed = 13f;
	public float height = 10f;
	public float distance = 18f;

	private Vector3 offsetX;
	private Vector3 offsetY;

	// Use this for initialization
	void Start () {
		offsetX = new Vector3 (0, height, distance);
		offsetY = new Vector3 (0, 0, distance);
	}
	
	// Update is called once per frame
	void Update () {
		offsetX = Quaternion.AngleAxis (speed * Time.deltaTime, Vector3.up) * offsetX;
		offsetY = Quaternion.AngleAxis (speed * Time.deltaTime, Vector3.right) * offsetY;
		transform.position = target.position + offsetX;
		transform.LookAt (target.position);
	}
}
