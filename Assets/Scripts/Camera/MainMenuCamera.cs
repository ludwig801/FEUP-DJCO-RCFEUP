using UnityEngine;
using System.Collections;

public class MainMenuCamera : MonoBehaviour {

	public Transform car;
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
		transform.position = car.position + offsetX;
		transform.LookAt (car.position);
	}
}
